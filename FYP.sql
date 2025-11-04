CREATE TABLE IMAGES (
    I_id INT IDENTITY(1,1) PRIMARY KEY,
    title NVARCHAR(255) NOT NULL,
    date NVARCHAR(50),
    path NVARCHAR(500) NOT NULL
);

CREATE TABLE PERSON (
    P_id INT IDENTITY(1,1) PRIMARY KEY,
    I_id INT NOT NULL,
    name NVARCHAR(255) NOT NULL,
    FOREIGN KEY (I_id) REFERENCES IMAGES(I_id) ON DELETE CASCADE
);

CREATE TABLE LOCATION (
    L_id INT IDENTITY(1,1) PRIMARY KEY,
    I_id INT NOT NULL,
    locationName NVARCHAR(255),
    FOREIGN KEY (I_id) REFERENCES IMAGES(I_id) ON DELETE CASCADE
);

CREATE TABLE EVENT (
    E_id INT IDENTITY(1,1) PRIMARY KEY,
    I_id INT NOT NULL,
    eventName NVARCHAR(255),
    FOREIGN KEY (I_id) REFERENCES IMAGES(I_id) ON DELETE CASCADE
);


CREATE TABLE faces (
    id INT PRIMARY KEY,
    person_name VARCHAR(255),
    embedding  VARCHAR(1000)
);


CREATE TABLE DUPLICATE (
    D_id INT IDENTITY(1,1) PRIMARY KEY, -- Primary key for the duplication record
    I_id INT NOT NULL,                 -- Foreign Key linking to the image (Required)
    title NVARCHAR(255),               -- Title (Optional, since it's already in IMAGES, but useful for lookups)
    SHA256 NVARCHAR(64) ,      -- Cryptographic hash for exact duplication check
    PHASH NVARCHAR(64) ,       -- Perceptual hash for near-duplicate check
    FOREIGN KEY (I_id) REFERENCES IMAGES(I_id) ON DELETE CASCADE
);




 delete from images where title='Screenshot 2025-10-30 182054'





 SELECT * FROM IMAGES
  SELECT * FROM PERSON 
 SELECT *FROM EVENT
 SELECT * FROM LOCATION
 SELECT * FROM DUPLICATE


 //******************************************************************************************//

 
  SELECT * FROM PERSON WHERE name='SAMEER'
 SELECT E.eventName FROM EVENT E ,PERSON P,IMAGES I WHERE E.I_id=P.I_id AND P.name='SAMEER'


 SELECT E.eventName,E.E_id
FROM EVENT E, PERSON P, IMAGES I 
WHERE E.I_id=P.I_id AND P.name='SAMEER'





 select * from photos

 SELECT * FROM faces
 DELETE FROM FACES WHERE PERSON_NAME='ASAD'















 delete from DUPLICATE
 DELETE FROM IMAGES
 DELETE FROM PERSON
 DELETE FROM EVENT
 DELETE FROM LOCATION