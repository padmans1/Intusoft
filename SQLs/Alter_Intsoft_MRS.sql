START TRANSACTION;
USE `dbName`;
DELIMITER $$
drop procedure if exists add_maskSettingsCol;
CREATE PROCEDURE add_maskSettingsCol() 
BEGIN
DECLARE colName TEXT;
SELECT column_name INTO colName
FROM information_schema.columns 
WHERE table_schema = 'intunewmodel'
    AND table_name = 'eye_fundus_image'
AND column_name = 'mask_settings';
IF colName is null THEN 
    ALTER TABLE  eye_fundus_image ADD  mask_settings TEXT ;
END IF; 
END$$
DELIMITER ;
CALL add_maskSettingsCol();
COMMIT;

DELIMITER $$
drop procedure if exists add_medicalHistoryCol;
CREATE PROCEDURE add_medicalHistoryCol() 
BEGIN
DECLARE colName TEXT;
SELECT column_name INTO colName
FROM information_schema.columns 
WHERE table_schema = 'intunewmodel'
    AND table_name = 'visit'
AND column_name = 'medical_history';
IF colName is null THEN 
    ALTER TABLE  visit ADD  medical_history TEXT ;
END IF; 
END$$
DELIMITER ;
CALL add_medicalHistoryCol();
COMMIT;


START TRANSACTION;
USE `dbName`;
DELIMITER $$
drop procedure if exists ChangeObservationTable;

CREATE PROCEDURE ChangeObservationTable() 
BEGIN
DECLARE colName TEXT;
SELECT column_name INTO colName
FROM information_schema.columns 
WHERE table_schema = 'intunewmodel'
    AND table_name = 'observation'
AND column_name = 'person_id';

IF colName is not null THEN 
   ALTER TABLE observation change COLUMN person_id patient_id bigint(20) not null;
END IF; 
END$$
DELIMITER ;
CALL ChangeObservationTable();
COMMIT;

START TRANSACTION;
USE `dbName`;
DELIMITER $$
drop procedure if exists ChangeObservationForeignKeyTable;

CREATE PROCEDURE ChangeObservationForeignKeyTable() 
BEGIN
DECLARE colName Text;
SELECT information_schema.TABLE_CONSTRAINTS.CONSTRAINT_NAME into colName FROM information_schema.TABLE_CONSTRAINTS 
WHERE information_schema.TABLE_CONSTRAINTS.CONSTRAINT_TYPE = 'FOREIGN KEY' 
AND information_schema.TABLE_CONSTRAINTS.TABLE_SCHEMA = 'intunewmodel'
And information_schema.TABLE_CONSTRAINTS.CONSTRAINT_NAME = 'observation_belongs_to_person_fk'
AND information_schema.TABLE_CONSTRAINTS.TABLE_NAME = 'observation';
if colName is not Null then
select colName;
ALTER TABLE `observation` 
DROP FOREIGN KEY `observation_belongs_to_person_fk`;  

ALTER TABLE `observation`  
ADD CONSTRAINT `observation_belongs_to_patient_fk` 
    FOREIGN KEY (`patient_id`) REFERENCES `patient` (`patient_id`) ON DELETE CASCADE; 
end if;
END$$
DELIMITER ;

CALL ChangeObservationForeignKeyTable();
COMMIT;


START TRANSACTION;
USE `dbName`;
DELIMITER $$
drop procedure if exists addPatDiagnosisTable;
CREATE PROCEDURE addPatDiagnosisTable() 
BEGIN
DECLARE colName INT;
SELECT count(table_name) INTO colName
FROM information_schema.columns 
WHERE table_schema = 'intunewmodel'
    AND table_name = 'patient_diagnosis';

IF colName = 0 THEN 
   CREATE TABLE `patient_diagnosis` (
  `patient_diagnosis_id` int(11) NOT NULL AUTO_INCREMENT,
  `patient_id` bigint(20) NOT NULL,
  `visit_id` bigint(20) NOT NULL,
  `concept_id` bigint(20) NOT NULL,
  `diagnosis_value_left` varchar(2000) DEFAULT NULL,
  `diagnosis_value_right` varchar(2000) DEFAULT NULL,
  `created_date` datetime DEFAULT NULL,
  `voided_date` datetime DEFAULT NULL,
  `last_modified_date` datetime DEFAULT NULL,
  `voided` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`patient_diagnosis_id`),
  KEY `fk_idx` (`patient_id`),
  KEY `patient_diagnosis_concept_id_idx` (`concept_id`),
  KEY `voided_fk_idx` (`voided`),
  CONSTRAINT `concept_id_fk` FOREIGN KEY (`concept_id`) REFERENCES `concept` (`concept_id`),
  CONSTRAINT `patient_diagnosis_pat_id` FOREIGN KEY (`patient_id`) REFERENCES `patient` (`patient_id`),
  CONSTRAINT `visit_id_fk` FOREIGN KEY (`visit_id`) REFERENCES `visit` (`visit_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
END IF; 
END$$
DELIMITER ;
CALL addPatDiagnosisTable() ;
COMMIT;

START TRANSACTION;
USE `dbName`;
INSERT INTO `concept` VALUES (12,'EyeFundusDiagnosis','','EYE FUNDUS DIAGNOSIS',0,3,4,'8a29dd87-b2ab-4a1a-8462-4ce57a9a1c26')
On duplicate key update concept_id ='12',short_name ='EyeFundusDiagnosis',description ='',fully_specified_name ='EYE FUNDUS DIAGNOSIS',is_set =0,datatype_id =3,class_id =4,uuid ='8a29dd87-b2ab-4a1a-8462-4ce57a9a1c26';
COMMIT;


START TRANSACTION;
USE `dbName` ;
ALTER TABLE `report` MODIFY COLUMN data_json LONGTEXT;
COMMIT;




