DROP USER DBMoonyFM;

ALTER SESSION SET "_ORACLE_SCRIPT" = TRUE;

---------------------- schema (tables' owner)
CREATE USER DBMoonyFM
IDENTIFIED BY Pa$$w0rd 
DEFAULT TABLESPACE Users 
QUOTA UNLIMITED ON Users;

GRANT CONNECT TO DBMoonyFM;
GRANT CREATE TABLE TO DBMoonyFM;
GRANT CREATE SEQUENCE TO DBMoonyFM;
GRANT CREATE VIEW TO DBMoonyFM;
GRANT CREATE INDEXTYPE TO DBMoonyFM;
GRANT CREATE PROCEDURE TO DBMoonyFM;
GRANT CREATE TRIGGER TO DBMoonyFM;
GRANT CREATE SESSION TO DBMoonyFM;
GRANT CREATE JOB TO DBMoonyFM;

GRANT EXECUTE ON sys.dbms_crypto TO DBMoonyFM;

COMMIT;

--------------------
