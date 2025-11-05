from fastapi import FastAPI, File, UploadFile, Form
from deepface import DeepFace
from pydantic import BaseModel
from wand.image import Image as WandImage
from PIL import Image as PILImage
import numpy as np
import json
import pyodbc
import cv2
import io
import hashlib
import os
from PIL import Image
import imagehash
import tempfile
from fastapi.responses import JSONResponse
from fastapi.staticfiles import StaticFiles
from fastapi.responses import JSONResponse
from pathlib import Path
import json
import xml.etree.ElementTree as ET
from datetime import datetime
from starlette.concurrency import run_in_threadpool
from fastapi import BackgroundTasks
import threading;
import traceback;


app = FastAPI()
# DB connection
def get_db_connection():
    return pyodbc.connect(
        'DRIVER={ODBC Driver 17 for SQL Server};'
        'SERVER=.;'
        'DATABASE=GEO PHOTO TAGGING;'
        'UID=sa;'
        'PWD=123'
    )


CHANGED_FILE = "changed_photos.json" 
DELETED_FILE = "deleted_photos.json" 

PHOTO_FOLDER = r"C:\Users\Dogesh\Desktop\PHOTO_SERVER"
identified_persons = ""
AUTO=False


# ----------------------------
# SHA256 Hash
# ----------------------------
def get_sha256(file_bytes: bytes) -> str:
    return hashlib.sha256(file_bytes).hexdigest()


# ----------------------------
# Perceptual Hash (pHash)
# ----------------------------
def get_phash(image_path: str) -> str:
    img = Image.open(image_path)
    return str(imagehash.phash(img))


# ----------------------------
# Add Photo
# ----------------------------
def add_photo_duplicatetable(file_path: str, title: str,image_id:int,cursor):
    with open(file_path, "rb") as f:
          file_bytes = f.read()

    sha256_hash = get_sha256(file_bytes)
    new_phash = get_phash(file_path)
    print("**********************STARTING INSERTED")
    try:
      
         print("**********************COONECTION BUILT ")
       
         print("**********************STARTING QUERY  ")
         cursor.execute(
            "INSERT INTO DUPLICATE (I_id, title, SHA256, PHASH) VALUES (?, ?, ?, ?)",
            (image_id, title, sha256_hash, new_phash)
        )
         print("********************** QUERY  EXECUTED ")
         print("**********************INSERTED")
    except Exception as e:
        print("💥 Database error:", e)
        return {"status": "error", "detail": str(e)}

    

def add_photo(file_path: str, title: str):
    with open(file_path, "rb") as f:
        file_bytes = f.read()

    sha256_hash = get_sha256(file_bytes)
    new_phash = get_phash(file_path)

    try:
        conn = get_db_connection()
        cursor = conn.cursor()

        # Step 1: SHA256 Exact Match
        cursor.execute("SELECT I_id, title, SHA256 FROM DUPLICATE WHERE SHA256 = ?", (sha256_hash,))
        row = cursor.fetchone()
        if row:
            print(f"[DUPLICATE] {file_path} → Exact match with {row.title} (SHA256)")
            return True

        # Step 2: Perceptual Hash Check
        cursor.execute("SELECT I_id, PHASH, title FROM DUPLICATE")
        for I_id, existing_phash, existing_title in cursor.fetchall():
            diff = imagehash.hex_to_hash(existing_phash) - imagehash.hex_to_hash(new_phash)
            if diff < 5:  # threshold for visual similarity
                print(f"[DUPLICATE] {file_path} → Visually similar to {existing_title} (diff={diff})")

                # still record it in DUPLICATE table so it's logged
                cursor.execute(
                    "INSERT INTO DUPLICATE (I_id, title, SHA256, PHASH) VALUES (?, ?, ?, ?)",
                    (I_id, title, sha256_hash, new_phash)
                )
                conn.commit()
                return True

        return False

    except Exception as e:
        print("💥 Database error:", e)
        return {"status": "error", "detail": str(e)}

    finally:
        # Connection ko har haal mein band karein
        if 'conn' in locals() and conn:
            conn.close()

@app.post("/DUPLICATE")
async def check_duplicate(file: UploadFile = File(...), title: str = Form(...)):
    try:
        file_bytes = await file.read()
        with tempfile.NamedTemporaryFile(delete=False, suffix=".jpg") as tmp:
            tmp.write(file_bytes)
            tmp_path = tmp.name

        duplicate = add_photo(tmp_path, title)
        os.remove(tmp_path)
        return {"duplicate": duplicate}

    except Exception as e:
        return {"error": str(e)}


# ----------------------------
# Face Recognition and Registration
# ----------------------------
class UpdateRequest(BaseModel):
    id: int
    name: str


class UpdateRequest2(BaseModel):
    OLDN: str
    name: str


def get_next_unknown_name(cursor):
    cursor.execute("SELECT person_name FROM faces WHERE person_name LIKE 'Unknown_%'")
    rows = cursor.fetchall()

    nums = []
    for r in rows:
        name = r[0]
        try:
            num = int(name.split("_")[1])
            nums.append(num)
        except:
            pass

    next_num = max(nums, default=0) + 1
    return f"Unknown_{next_num}"

def identify_or_register(image_path, threshold=0.6):
    # 1. Get embeddings for all faces in image
    embeddings = DeepFace.represent(
        img_path=image_path,
        model_name="ArcFace",
        detector_backend="retinaface",
        enforce_detection=False
    )

    if not embeddings:
        print("No face detected in image.")
        return []

    try:
        conn = get_db_connection()
        cursor = conn.cursor()

        # 2. Fetch all saved embeddings once
        cursor.execute("SELECT id, person_name, embedding FROM faces")
        rows = cursor.fetchall()

        results = []

        # 3. Compare each detected face embedding with DB
        for emb_data in embeddings:
            embedding = np.array(emb_data["embedding"])

            best_match = None
            best_distance = float("inf")

            for person_id, person_name, emb_json in rows:
                db_embedding = np.array(json.loads(emb_json))

                # --- Cosine similarity distance ---
                dot = np.dot(db_embedding, embedding)
                norm = np.linalg.norm(db_embedding) * np.linalg.norm(embedding)
                distance = 1 - (dot / norm)  # 0 = perfect, 1 = completely different

                print(f"Comparing with {person_name} → distance={distance:.3f}")

                if distance < best_distance:
                    best_distance = distance
                    best_match = person_name
                    matched_id = person_id

            # 4. Threshold check
            if best_distance < threshold:
                print(f"✅ Matched with {best_match}, distance={best_distance:.3f}")
                results.append({
                    "id": matched_id,
                    "name": best_match
                })
            else:
                new_name = get_next_unknown_name(cursor)
                cursor.execute(
                    "INSERT INTO faces (person_name, embedding) VALUES (?, ?)",
                    (new_name, json.dumps(embedding.tolist()))
                )
                conn.commit()
                cursor.execute("SELECT @@IDENTITY")
                new_id = cursor.fetchone()[0]
                print(f"🆕 No match found → Saved as {new_name}")
                results.append({
                    "id": new_id,
                    "name": new_name
                })

        return results

    except Exception as e:
        print("💥 Database error:", e)
        return {"status": "error", "detail": str(e)}

    finally:
        # Connection ko har haal mein band karein
        if 'conn' in locals() and conn:
            conn.close()


@app.post("/identify")
async def identify(file: UploadFile = File(...)):
    # bytes → numpy image
    file_bytes = await file.read()
    with tempfile.NamedTemporaryFile(delete=False, suffix=".jpg") as tmp:
        tmp.write(file_bytes)
        tmp_path = tmp.name

    # 2. Run face recognition
    results = identify_or_register(tmp_path)

    # delete temp file
    os.remove(tmp_path)
    return {
        "results": results
    }


@app.post("/update_persons")
async def update_persons(req: UpdateRequest):
    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute("UPDATE faces SET person_name=? WHERE id=?", (req.name, req.id))
        conn.commit()
        return {"status": "success", "id": req.id, "new_name": req.name}

    except Exception as e:
        print("💥 Database error:", e)
        return {"status": "error", "detail": str(e)}

    finally:
        # Connection ko har haal mein band karein
        if 'conn' in locals() and conn:
            conn.close()



@app.post("/update_persons2")
async def update_persons(req: UpdateRequest2):
    print("🔥 Received:", req)
    try:
        conn = get_db_connection()
        cursor = conn.cursor()
        cursor.execute("UPDATE faces SET person_name=? WHERE person_name=?", (req.name, req.OLDN))
        conn.commit()
        return {"status": "success", "old": req.OLDN, "new_name": req.name}

    except Exception as e:
        print("💥 Database error:", e)
        return {"status": "error", "detail": str(e)}

    finally:
        # Connection ko har haal mein band karein
        if 'conn' in locals() and conn:
            conn.close()


@app.post("/upload_photo")
async def upload_photo(file: UploadFile = File(...), metadata: str = Form(None)):
    try:
        server_folder = PHOTO_FOLDER
        os.makedirs(server_folder, exist_ok=True)

        filename = os.path.basename(file.filename)
        save_path = os.path.join(server_folder, filename)
        add_changed_photo(save_path)

        # Save the uploaded file first
        contents = await file.read()
        with open(save_path, "wb") as f:
            f.write(contents)

        # ✅ Inject metadata as XMP (same structure as C#)
        if metadata:
            meta = json.loads(metadata)
            metadata_json = json.dumps(meta)

            xmp = f"""<?xpacket begin='﻿' id='W5M0MpCehiHzreSzNTczkc9d'?>
<x:xmpmeta xmlns:x='adobe:ns:meta/'>
  <rdf:RDF xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#'>
    <rdf:Description xmlns:custom='https://yourdomain/custom/'>
      <custom:PhotoInfo>{metadata_json}</custom:PhotoInfo>
    </rdf:Description>
  </rdf:RDF>
</x:xmpmeta>
<?xpacket end='w'?>"""

            # Write XMP profile to image
            with WandImage(filename=save_path) as img:
                img.profiles["xmp"] = xmp.encode("utf-8")
                img.save(filename=save_path)

        print(f"✅ Saved photo with metadata: {save_path}")
        return {
            "status": "success",
            "path": save_path,
            "metadata": meta if metadata else "none"
        }

    except Exception as e:
        return {"status": "error", "message": str(e)}

@app.post("/update_metadata")
async def update_metadata(filename: str = Form(...), metadata: str = Form(...)):
    path = os.path.join(PHOTO_FOLDER, filename)
    add_changed_photo(path)
    if not os.path.exists(path):
        return {"error": "file not found"}

    try:
        meta = json.loads(metadata)
        metadata_json = json.dumps(meta)

        xmp = f"""<?xpacket begin='﻿' id='W5M0MpCehiHzreSzNTczkc9d'?>
<x:xmpmeta xmlns:x='adobe:ns:meta/'>
  <rdf:RDF xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#'>
    <rdf:Description xmlns:custom='https://yourdomain/custom/'>
      <custom:PhotoInfo>{metadata_json}</custom:PhotoInfo>
    </rdf:Description>
  </rdf:RDF>
</x:xmpmeta>
<?xpacket end='w'?>"""

        from wand.image import Image as WandImage
        with WandImage(filename=path) as img:
            img.profiles["xmp"] = xmp.encode("utf-8")
            img.save(filename=path)

        return {"status": "updated", "file": filename}

    except Exception as e:
        return {"error": str(e)}


@app.get("/get_all_photos")
def get_all_photos():
    exts = (".jpg", ".jpeg", ".png", ".bmp")
    photos = []
    for root, _, files in os.walk(PHOTO_FOLDER):
        for f in files:
            if f.lower().endswith(exts):
                photos.append(f)
    return JSONResponse(content={"photos": photos})


# Serve images so C# can download them by URL
app.mount("/photos", StaticFiles(directory=PHOTO_FOLDER), name="photos")


@app.get("/get_metadata")
async def get_metadata(filename: str):
    path = os.path.join(PHOTO_FOLDER, filename)
    if not os.path.exists(path):
        return {"error": "file not found"}

    try:
        with WandImage(filename=path) as img:
            xmp_profile = img.profiles.get("xmp")
            if not xmp_profile:
                return {"error": "no xmp data found"}

            xmp_data = xmp_profile.decode("utf-8", errors="ignore")

            # Parse the XMP XML
            root = ET.fromstring(xmp_data)
            ns = {"custom": "https://yourdomain/custom/"}

            element = root.find(".//custom:PhotoInfo", ns)
            if element is None:
                return {"error": "No PhotoInfo found inside XMP"}

            metadata_json = element.text
            data = json.loads(metadata_json)
            return data

    except Exception as e:
        return JSONResponse({"error": str(e)})


@app.post("/delete_photo")
async def delete_photo(filename: str = Form(...), title: str = Form(...)):
    path = os.path.join(PHOTO_FOLDER, filename)
    add_deleted_photo(filename)
    if not os.path.exists(path):
        return {"error": "file not found"}

    conn = None
    try:
        conn =get_db_connection()
        cursor = conn.cursor()

        # ✅ Delete from DB
        cursor.execute("DELETE FROM IMAGES WHERE title = ?", (title,))
        conn.commit()
    

        # ✅ Delete from filesystem
        os.remove(path)
        print(f"🗑 Deleted {filename} from server and database.")
        return {"status": "deleted", "file": filename}

    except Exception as e:
        print(f"💥 Error deleting {filename}: {e}")
        return {"error": str(e)}

    finally:
        if conn:
            conn.close()

@app.post("/upload_photo_MODEL")
async def upload_photo_MODEL(
    file: UploadFile = File(...),
    metadata: str = Form(None),
    title: str = Form(...),
    date: str = Form(...),
    path: str = Form(...)
):
    server_folder = PHOTO_FOLDER
    os.makedirs(server_folder, exist_ok=True)

    filename = os.path.basename(file.filename)
    save_path = os.path.join(server_folder, filename)
    add_changed_photo(save_path)

    contents = await file.read()
    with open(save_path, "wb") as f:
        f.write(contents)

    meta = json.loads(metadata) if metadata else {}

    # Make sure all expected keys exist
    meta.setdefault("Person", "")
    meta.setdefault("Location", "")
    meta.setdefault("Event", "")

    # Launch DeepFace + metadata in background
   # run_in_process(save_path, meta, title, date, path)
    threading.Thread(target=process_deepface_and_metadata, args=(save_path, meta, title, date, path), daemon=True).start()


    return {"status": "processing", "file": filename}



def process_deepface_and_metadata(save_path, meta, title, date, path):
    try:
        print(f"🧠 Background started for {os.path.basename(save_path)}")

        results = identify_or_register(save_path)
        if results:
            meta["Person"] = ", ".join(r["name"] for r in results)
        else:
            meta["Person"] = "Unknown"

        print(f"✅ DeepFace done — Person={meta['Person']}")

       # Insert metadata into DB
        add_photo_to_db(
            title,
            date,
            path,
            persons=meta['Person'],
            locations="",
            events=""
        )

        print(f"🗃️ DB insert completed for {os.path.basename(save_path)}")

        # Write metadata into photo (XMP)
        metadata_json = json.dumps(meta)
        xmp = f"""<?xpacket begin='﻿' id='W5M0MpCehiHzreSzNTczkc9d'?>
<x:xmpmeta xmlns:x='adobe:ns:meta/'>
  <rdf:RDF xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#'>
    <rdf:Description xmlns:custom='https://yourdomain/custom/'>
      <custom:PhotoInfo>{metadata_json}</custom:PhotoInfo>
    </rdf:Description>
  </rdf:RDF>
</x:xmpmeta>
<?xpacket end='w'?>"""

        try:
            with WandImage(filename=save_path) as img:
                img.profiles["xmp"] = xmp.encode("utf-8")
                img.save(filename=save_path)
            print(f"🖋️ Metadata written successfully for {os.path.basename(save_path)}")
        except Exception as img_error:
            print(f"💥 Failed to write metadata for {save_path}: {img_error}")

        print(f"✅ All done: {os.path.basename(save_path)} | {meta['Person']}")

    except Exception as e:
        print(f"💥 Background processing failed for {save_path}: {e}")
        print(traceback.format_exc())


# def run_in_process(save_path, meta, title, date, path):
#     proc = multiprocessing.Process(
#         target=process_deepface_and_metadata,
#         args=(save_path, meta, title, date, path)
#     )
#     proc.start()


def add_photo_to_db(title, date, path, persons=None, locations=None, events=None):
  
    try:
        conn_local = get_db_connection()
        cursor = conn_local.cursor()

        cursor.execute("""
            INSERT INTO IMAGES (title, date, path)
            OUTPUT INSERTED.I_id
            VALUES (?, ?, ?)
        """, (title, date, path))
        image_id = cursor.fetchone()[0]

        # Insert person(s)
        if persons:
            for name in [p.strip() for p in persons.split(",") if p.strip()]:
                cursor.execute("INSERT INTO PERSON (I_id, name) VALUES (?, ?)", (image_id, name))

        # Insert location
        if locations:
            cursor.execute("INSERT INTO LOCATION (I_id, locationName) VALUES (?, ?)", (image_id, locations))

        # Insert event
        if events:
            cursor.execute("INSERT INTO EVENT (I_id, eventName) VALUES (?, ?)", (image_id, events))

        #add_photo(path, title)
        add_photo_duplicatetable(path, title,image_id,cursor)
        conn_local.commit()

    except Exception as e:
        print(f"💥 DB insert failed: {e}")
        if conn_local:
            conn_local.rollback()
    finally:
        if conn_local:
            conn_local.close()


# @app.post("/upload_photo_MODEL")
# async def upload_photo_MODEL(
#     file: UploadFile = File(...),
#     metadata: str = Form(None),
#     title: str = Form(...),
#     date: str = Form(...),
#     path: str = Form(...)
# ):
#     server_folder = PHOTO_FOLDER
#     os.makedirs(server_folder, exist_ok=True)

#     filename = os.path.basename(file.filename)
#     save_path = os.path.join(server_folder, filename)

#     contents = await file.read()
#     with open(save_path, "wb") as f:
#         f.write(contents)

#     meta = json.loads(metadata) if metadata else {}

#     # 🔥 Launch DeepFace + metadata in isolated process
#     run_in_process(save_path, meta, title, date, path)

#     return {"status": "processing", "file": filename}


# def process_deepface_and_metadata(save_path, meta, title, date, path):
#     try:
#         results = identify_or_register(save_path)
#         if results:
#             meta["Person"] = ", ".join(r["name"] for r in results)
#         else:
#             meta["Person"] = "Unknown"

#         add_photo_to_db(title, date, path, meta["Person"])

#         metadata_json = json.dumps(meta)
#         xmp = f"""<?xpacket begin='�' id='W5M0MpCehiHzreSzNTczkc9d'?>
# <x:xmpmeta xmlns:x='adobe:ns:meta/'>
#   <rdf:RDF xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#'>
#     <rdf:Description xmlns:custom='https://yourdomain/custom/'>
#       <custom:PhotoInfo>{metadata_json}</custom:PhotoInfo>
#     </rdf:Description>
#   </rdf:RDF>
# </x:xmpmeta>
# <?xpacket end='w'?>"""

#         with WandImage(filename=save_path) as img:
#             img.profiles["xmp"] = xmp.encode("utf-8")
#             img.save(filename=save_path)

      
#         print(f"✅ Processed in background: {os.path.basename(save_path)} | {meta['Person']}")

#     except Exception as e:
#         print(f"💥 Background processing failed for {save_path}: {e}")


# def run_in_process(save_path, meta, title, date, path):
#     proc = multiprocessing.Process(
#         target=process_deepface_and_metadata,
#         args=(save_path, meta, title, date, path)
#     )
#     proc.start()


# def add_photo_to_db(title, date, path, persons, locations=None, events=None):
#     """Insert image + related metadata into database"""
#     conn_local = None
#     try:
#         conn_local = get_db_connection()
#         cursor = conn_local.cursor()

#         cursor.execute("""
#             INSERT INTO IMAGES (title, date, path)
#             OUTPUT INSERTED.I_id
#             VALUES (?, ?, ?)
#         """, (title, date, path))

#         image_id = cursor.fetchone()[0]

#         # Insert persons
#         if persons:
#             names = [p.strip() for p in persons.split(",") if p.strip()]
#             for name in names:
#                 cursor.execute("INSERT INTO PERSON (I_id, name) VALUES (?, ?)", (image_id, name))

#         # Insert location
#         if locations:
#             cursor.execute("INSERT INTO LOCATION (I_id, locationName) VALUES (?, ?)", (image_id, locations))

#         # Insert event
#         if events:
#             cursor.execute("INSERT INTO EVENT (I_id, eventName) VALUES (?, ?)", (image_id, events))

#         add_photo(path, title)
#         conn_local.commit()

#     except Exception as e:
#         print(f"💥 DB insert failed: {e}")
#         conn_local.rollback()

#     finally:
#         if conn_local:
#             conn_local.close()



# @app.post("/upload_photo_MODEL")
# async def upload_photo_MODEL(
#     background_tasks: BackgroundTasks,
#     file: UploadFile = File(...),
#     metadata: str = Form(None)
# ):
#     server_folder = PHOTO_FOLDER
#     os.makedirs(server_folder, exist_ok=True)
#     filename = os.path.basename(file.filename)
#     save_path = os.path.join(server_folder, filename)

#     # Save uploaded file first
#     contents = await file.read()
#     with open(save_path, "wb") as f:
#         f.write(contents)

#     # Parse metadata if present
#     meta = json.loads(metadata) if metadata else {}

#     # Respond immediately
#     background_tasks.add_task(process_deepface_and_metadata, save_path, meta)
#     return {"status": "processing", "file": filename}


# def process_deepface_and_metadata(save_path, meta):
#     try:
#         results = identify_or_register(save_path)
#         if results:
#             meta["Person"] = ", ".join(r["name"] for r in results)
#         else:
#             meta["Person"] = "Unknown"

#         metadata_json = json.dumps(meta)
#         xmp = f"""<?xpacket begin='�' id='W5M0MpCehiHzreSzNTczkc9d'?>
# <x:xmpmeta xmlns:x='adobe:ns:meta/'>
#   <rdf:RDF xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#'>
#     <rdf:Description xmlns:custom='https://yourdomain/custom/'>
#       <custom:PhotoInfo>{metadata_json}</custom:PhotoInfo>
#     </rdf:Description>
#   </rdf:RDF>
# </x:xmpmeta>
# <?xpacket end='w'?>"""

#         with WandImage(filename=save_path) as img:
#             img.profiles["xmp"] = xmp.encode("utf-8")
#             img.save(filename=save_path)

#         add_photo(save_path)
#         print(f"✅ Processed in background: {os.path.basename(save_path)} | {meta['Person']}")

#     except Exception as e:
#         print(f"💥 Background processing failed for {save_path}: {e}")


# @app.post("/upload_photo_MODEL")
# async def upload_photo_MODEL(file: UploadFile = File(...), metadata: str = Form(None)):
#     try:
#         server_folder = PHOTO_FOLDER
#         os.makedirs(server_folder, exist_ok=True)
#         filename = os.path.basename(file.filename)
#         save_path = os.path.join(server_folder, filename)

#         contents = await file.read()
#         with open(save_path, "wb") as f:
#             f.write(contents)

#         # --- Parse metadata JSON ---
#         meta = json.loads(metadata) if metadata else {}

#         # ✅ Run DeepFace in separate thread so server stays responsive
#         try:
#             results = await run_in_threadpool(identify_or_register, save_path)
#             if results and len(results) > 0:
#                 detected_names = [r["name"] for r in results]
#                 meta["Person"] = ", ".join(detected_names)
#             else:
#                 meta["Person"] = "Unknown"
#         except Exception as e:
#             print(f"⚠ DeepFace error on {filename}: {e}")
#             meta["Person"] = "Unknown"

#         # --- Inject metadata ---
#         metadata_json = json.dumps(meta)
#         xmp = f"""<?xpacket begin='�' id='W5M0MpCehiHzreSzNTczkc9d'?>
# <x:xmpmeta xmlns:x='adobe:ns:meta/'>
#   <rdf:RDF xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#'>
#     <rdf:Description xmlns:custom='https://yourdomain/custom/'>
#       <custom:PhotoInfo>{metadata_json}</custom:PhotoInfo>
#     </rdf:Description>
#   </rdf:RDF>
# </x:xmpmeta>
# <?xpacket end='w'?>"""

#         with WandImage(filename=save_path) as img:
#             img.profiles["xmp"] = xmp.encode("utf-8")
#             img.save(filename=save_path)

#         # --- Insert into DB ---
#         await run_in_threadpool(add_photo, save_path)

#         print(f"✅ Saved & analyzed photo: {filename} | Persons: {meta['Person']}")
#         return {
#             "status": "success",
#             "file": filename,
#             "recognized_persons": meta["Person"],
#             "metadata": meta
#         }

#     except Exception as e:
#         return {"status": "error", "message": str(e)}

# @app.post("/upload_photo_MODEL")
# async def upload_photo_MODEL(
#     background_tasks: BackgroundTasks,
#     file: UploadFile = File(...),
#     metadata: str = Form(None)
# ):
#     server_folder = PHOTO_FOLDER
#     os.makedirs(server_folder, exist_ok=True)
#     filename = os.path.basename(file.filename)
#     save_path = os.path.join(server_folder, filename)

#     # Save uploaded file first
#     contents = await file.read()
#     with open(save_path, "wb") as f:
#         f.write(contents)

#     # Start DeepFace in background (non-blocking)
#     background_tasks.add_task(identify_or_register, save_path)

#     return {"status": "processing", "filename": filename}

@app.post("/insert_into_database")
async def save(
    title: str = Form(...),
    date: str = Form(...),
    path: str = Form(...),
    persons: str = Form(""),
    locations: str = Form(""),
    events: str = Form("")
):
    print("🔥 Received metadata for insertion:", title, date, path, persons, locations, events)
    try:
        conn = get_db_connection()
        cursor = conn.cursor()

        # Step 1️⃣ — Insert into IMAGES table
        insert_image_query = """
            INSERT INTO IMAGES (title, date, path)
            OUTPUT INSERTED.I_id
            VALUES (?, ?, ?)
        """
        cursor.execute(insert_image_query, (title, date, path))
        image_id = cursor.fetchone()[0]  # Get the new image ID

        # Step 2️⃣ — Insert PERSON records (comma-separated)
        if persons.strip():
            for person in [p.strip() for p in persons.split(",") if p.strip()]:
                cursor.execute(
                    "INSERT INTO PERSON (I_id, name) VALUES (?, ?)",
                    (image_id, person)
                )

        # Step 3️⃣ — Insert LOCATION records (comma-separated)
        if locations.strip():
            for loc in [l.strip() for l in locations.split(",") if l.strip()]:
                cursor.execute(
                    "INSERT INTO LOCATION (I_id, locationName) VALUES (?, ?)",
                    (image_id, loc)
                )

        # Step 4️⃣ — Insert EVENT records (comma-separated)
        if events.strip():
            for ev in [e.strip() for e in events.split(",") if e.strip()]:
                cursor.execute(
                    "INSERT INTO EVENT (I_id, eventName) VALUES (?, ?)",
                    (image_id, ev)
                )
        print(f"Inserted image ID: {image_id} IN IMAGES" )
        add_photo_duplicatetable(path, title,image_id,cursor)
        print(f" DUPLICATE Inserted image ID: {image_id} IN IMAGES" )
        conn.commit()
        return {"message": "Metadata saved successfully!", "image_id": image_id}

    except Exception as e:
        return {"error": str(e)}

    finally:
        if conn:
            conn.close()


@app.post("/update_database")
async def update(
    title: str = Form(...),
    date: str = Form(...),
    path: str = Form(...),
    persons: str = Form(""),
    locations: str = Form(""),
    events: str = Form("")
):
    try:
        conn =get_db_connection()
        cursor = conn.cursor()

        # Step 1️⃣ — Find image ID by title
        cursor.execute("SELECT I_id FROM IMAGES WHERE title = ?", (title,))
        row = cursor.fetchone()
        if not row:
            return {"error": f"No image found with title '{title}'"}

        image_id = row[0]

        # Clear existing data
        cursor.execute("DELETE FROM PERSON WHERE I_id = ?", (image_id,))
        cursor.execute("DELETE FROM LOCATION WHERE I_id = ?", (image_id,))
        cursor.execute("DELETE FROM EVENT WHERE I_id = ?", (image_id,))

        # Step 2️⃣ — Re-insert PERSON records
        if persons.strip():
            for person in [p.strip() for p in persons.split(",") if p.strip()]:
                cursor.execute(
                    "INSERT INTO PERSON (I_id, name) VALUES (?, ?)",
                    (image_id, person)
                )

        # Step 3️⃣ — Re-insert LOCATION records
        if locations.strip():
            for loc in [l.strip() for l in locations.split(",") if l.strip()]:
                cursor.execute(
                    "INSERT INTO LOCATION (I_id, locationName) VALUES (?, ?)",
                    (image_id, loc)
                )

        # Step 4️⃣ — Re-insert EVENT records
        if events.strip():
            for ev in [e.strip() for e in events.split(",") if e.strip()]:
                cursor.execute(
                    "INSERT INTO EVENT (I_id, eventName) VALUES (?, ?)",
                    (image_id, ev)
                )

        conn.commit()
        return {"message": "Metadata updated successfully!", "image_id": image_id}

    except Exception as e:
        return {"error": str(e)}

    finally:
        if conn:
            conn.close()















# ... (Baaki code)


def add_changed_photo(absolute_path):
    # 💥 CRITICAL FIX: Extract relative path or just the filename
    
    # 1. Filename extraction (agar aapki saari photos ROOT_DIR mein hain):
    relative_path = os.path.basename(absolute_path) # Sirf "WhatsApp Image 2025-..." milega
    
    # 2. Agar photos sub-folders mein hain, toh relative path nikaalein:
    # relative_path = os.path.relpath(absolute_path, ROOT_DIR) # "folder/photo.jpg" milega
    
    filename_to_store = relative_path # Ya to basename ya relative path
    
    try:
        with open(CHANGED_FILE, "r") as f:
            data = json.load(f)
    except:
        data = []

    if filename_to_store not in data: # Check only the filename/relative path
        data.append(filename_to_store)

    with open(CHANGED_FILE, "w") as f:
        json.dump(data, f)
# Is function ko call karte waqt ensure karein ki aapne yahan path correction kiya hai.




@app.get("/changed_photos")
def get_changed_photos():
    try:
        with open(CHANGED_FILE, "r") as f:
            data = json.load(f)
        return data
    except:
        return []
    

# ... (existing imports and code)

@app.post("/clear_changed_photos")
def clear_changed_photos():
    """Server-side list of changed photos ko reset karta hai."""
    global CHANGED_FILE
    try:
        # File ko khaali list se overwrite kar dein
        with open(CHANGED_FILE, "w") as f:
            json.dump([], f)
        print("✅ CHANGED_FILE list cleared.")
        return {"status": "success", "message": "Changed photos list cleared."}
    except Exception as e:
        print(f"💥 Error clearing CHANGED_FILE: {e}")
        return {"status": "error", "message": str(e)}

# ... (rest of your FastAPI code)


def add_deleted_photo(filename: str):
    """
    Jab koi file delete hoti hai, toh yeh function uska filename DELETED_FILE mein add karta hai.
    (Isko aapke file monitoring system se call hona chahiye)
    """
    try:
        if os.path.exists(DELETED_FILE):
            with open(DELETED_FILE, "r") as f:
                data = json.load(f)
        else:
            data = []
    except json.JSONDecodeError:
        data = []

    # Ensure ki path sirf filename ho, jaisa ki aapke client mein sync hota hai
    # agar aapka monitoring absolute path de raha hai toh use Path.basename se theek karein.
    base_filename = os.path.basename(filename) 
    
    if base_filename not in data:
        data.append(base_filename)

    with open(DELETED_FILE, "w") as f:
        json.dump(data, f)
    print(f"✅ Logged deletion: {base_filename}")


# --- API ENDPOINTS FOR DELETION SYNC ---

@app.get("/deleted_photos")
def get_deleted_photos():
    """
    Client ko deleted photos ki list (filenames) bhejta hai.
    """
    try:
        if os.path.exists(DELETED_FILE):
            with open(DELETED_FILE, "r") as f:
                data = json.load(f)
            return data
        else:
            return []
    except json.JSONDecodeError:
        print(f"⚠️ Warning: {DELETED_FILE} is corrupted or empty.")
        return []
    except Exception as e:
        print(f"💥 Error reading deleted photos file: {e}")
        return {"status": "error", "message": str(e)}

@app.post("/clear_deleted_photos")
def clear_deleted_photos():
    """
    Client sync hone ke baad server se deletion list ko reset karta hai.
    """
    try:
        # File ko khaali list se overwrite kar dein
        with open(DELETED_FILE, "w") as f:
            json.dump([], f)
        print("✅ DELETED_FILE list cleared.")
        return {"status": "success", "message": "Deleted photos list cleared."}
    except Exception as e:
        print(f"💥 Error clearing DELETED_FILE: {e}")
        return {"status": "error", "message": str(e)}
