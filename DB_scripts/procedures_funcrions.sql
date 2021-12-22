ALTER SESSION SET "_ORACLE_SCRIPT" = TRUE;
SELECT * FROM user_procedures;

BEGIN
   REGISTER_USER('moony', '1111');
END;

UPDATE user_table SET user_role = 2 WHERE user_login = 'MOONY';
select * from user_table;

-------------------------ENCRYPTION_PASSWORD-------------------------

CREATE OR REPLACE FUNCTION encryption_password
    (p_user_password IN user_table.user_password%TYPE)
    RETURN user_table.user_password%TYPE
IS
    l_key VARCHAR2(2000) := '0710196810121972';
    l_in_val VARCHAR2(2000) := p_user_password;
    l_mod NUMBER := DBMS_CRYPTO.encrypt_aes128 + DBMS_CRYPTO.chain_cbc + DBMS_CRYPTO.pad_pkcs5;
    l_enc RAW(2000);
BEGIN
    l_enc := DBMS_CRYPTO.encrypt(utl_i18n.string_to_raw(l_in_val, 'AL32UTF8'), l_mod, utl_i18n.string_to_raw(l_key, 'AL32UTF8'));
RETURN RAWTOHEX(l_enc);
END encryption_password;

-------------------------DECRYPTION_PASSWORD-------------------------

CREATE OR REPLACE FUNCTION decryption_password
    (p_user_password IN user_table.user_password%TYPE)
    RETURN user_table.user_password%TYPE
IS
    l_key VARCHAR2(2000) := '0710196810121972';
    l_in_val RAW(2000) := HEXTORAW(p_user_password);
    l_mod NUMBER := DBMS_CRYPTO.encrypt_aes128 + DBMS_CRYPTO.chain_cbc + DBMS_CRYPTO.pad_pkcs5;
    l_dec RAW(2000);
BEGIN
    l_dec := DBMS_CRYPTO.decrypt(l_in_val, l_mod, utl_i18n.string_to_raw(l_key, 'AL32UTF8'));
RETURN utl_i18n.raw_to_char(l_dec);
END decryption_password;

-------------------------REGISTER_USER-------------------------

CREATE OR REPLACE PROCEDURE register_user
    (p_user_login IN user_table.user_login%TYPE,
    p_user_password IN user_table.user_password%TYPE)
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM user_table WHERE UPPER(user_login) = UPPER(p_user_login);
    IF (cnt = 0) THEN
        INSERT INTO user_table(user_login, user_password, user_role) VALUES(UPPER(p_user_login), encryption_password(UPPER(p_user_password)), 1);
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20001, 'This login is already taken');
    END IF;
END register_user;

-------------------------CHECK_ROLE-------------------------

CREATE OR REPLACE PROCEDURE check_role
    (p_user_login IN user_table.user_login%TYPE,
    o_user_role OUT role_table.role_name%TYPE)
IS
    CURSOR role_cursor IS SELECT role_name FROM user_role_view WHERE UPPER(user_login) = UPPER(p_user_login);
BEGIN
    OPEN role_cursor;
        FETCH role_cursor INTO o_user_role;
        IF role_cursor%NOTFOUND THEN
            RAISE_APPLICATION_ERROR(-20004, 'Role error');
        END IF;
    CLOSE role_cursor;
END check_role;

-------------------------LOG_IN_USER-------------------------

CREATE OR REPLACE PROCEDURE log_in_user
    (p_user_login IN user_table.user_login%TYPE,
    p_user_password IN user_table.user_password%TYPE,
    o_user_id OUT user_table.user_id%TYPE,
    o_user_login OUT user_table.user_login%TYPE,
    o_user_role OUT role_table.role_name%TYPE)
IS
    CURSOR user_cursor IS SELECT user_id, user_login FROM user_table WHERE UPPER(user_login) = UPPER(p_user_login) AND user_password = encryption_password(UPPER(p_user_password));
BEGIN
    OPEN user_cursor;
        FETCH user_cursor INTO o_user_id, o_user_login;
        IF user_cursor%NOTFOUND THEN
            RAISE_APPLICATION_ERROR(-20002, 'Incorrect login or password');
        END IF;
    CLOSE user_cursor;
    check_role(o_user_login, o_user_role);
END log_in_user;

-------------------------SEARCH_USER-------------------------

CREATE OR REPLACE PROCEDURE search_user
    (p_user_login IN user_table.user_login%TYPE,
    o_user_login OUT user_table.user_login%TYPE,
    o_user_password OUT user_table.user_password%TYPE)
IS
    CURSOR user_cursor IS SELECT user_login, decryption_password(user_password) FROM user_table WHERE UPPER(user_login) = UPPER(p_user_login);
BEGIN
    OPEN user_cursor;
        FETCH user_cursor INTO o_user_login, o_user_password;
        IF user_cursor%NOTFOUND THEN
            RAISE_APPLICATION_ERROR(-20003, 'User is not found');
        END IF;
    CLOSE user_cursor;
END search_user;

-------------------------UPDATE_USER_LOGIN-------------------------

CREATE OR REPLACE PROCEDURE update_user_login
    (p_user_login IN user_table.user_login%TYPE,
    p_new_user_login IN user_table.user_login%TYPE)
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM user_table WHERE UPPER(user_login) = UPPER(p_user_login);
    IF (cnt != 0) THEN
        SELECT COUNT(*) INTO cnt FROM user_table WHERE UPPER(user_login) = UPPER(p_new_user_login);
        IF (cnt = 0) THEN
            UPDATE user_table SET user_login = UPPER(p_new_user_login) WHERE UPPER(user_login) = UPPER(p_user_login);
            COMMIT;
        ELSE
            RAISE_APPLICATION_ERROR(-20001, 'This login is already taken');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20003, 'User is not found');
    END IF;
END update_user_login;

-------------------------UPDATE_USER_PASSWORD-------------------------

CREATE OR REPLACE PROCEDURE update_user_password
    (p_user_login IN user_table.user_login%TYPE,
    p_new_user_password IN user_table.user_password%TYPE)
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM user_table WHERE UPPER(user_login) = UPPER(p_user_login);
    IF (cnt != 0) THEN
        UPDATE user_table SET user_password = encryption_password(UPPER(p_new_user_password)) WHERE UPPER(user_login) = UPPER(p_user_login);
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20003, 'User is not found');
    END IF;
END update_user_password;

-------------------------DELETE_USER-------------------------

CREATE OR REPLACE PROCEDURE delete_user
    (p_user_login IN user_table.user_login%TYPE)
IS
    cnt NUMBER;
    usr_id user_table.user_id%TYPE;
BEGIN
    SELECT COUNT(*) INTO cnt FROM user_table WHERE UPPER(user_login) = UPPER(p_user_login);
    IF (cnt != 0) THEN
        SELECT user_id INTO usr_id FROM user_table WHERE UPPER(user_login) = UPPER(p_user_login);
        DELETE FROM user_table WHERE UPPER(user_table.user_login) = UPPER(p_user_login);
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20003, 'User is not found');
    END IF;
END delete_user;

-------------------------CREATE_ARTIST-------------------------

CREATE OR REPLACE PROCEDURE create_artist
    (p_artist_name IN artist_table.artist_name%TYPE)
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM artist_table WHERE UPPER(artist_name) = UPPER(p_artist_name);
    IF (cnt = 0) THEN
        INSERT INTO artist_table(artist_name) VALUES(UPPER(p_artist_name));
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20005, 'Artist with this name already exists');
    END IF;
END create_artist;

-------------------------CREATE_ALBUM (without blob)-------------------------

CREATE OR REPLACE PROCEDURE create_album
    (p_artist_name IN artist_table.artist_name%TYPE,
    p_album_name IN album_table.album_name%TYPE,
    p_album_released IN album_table.album_released%TYPE)
IS
    artist_id artist_table.artist_id%TYPE;
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM artist_album_view WHERE UPPER(album_name) = UPPER(p_album_name) AND UPPER(artist_name) = UPPER(p_artist_name);
    IF (cnt = 0) THEN
        SELECT artist_id INTO artist_id FROM artist_table WHERE UPPER(artist_name) = UPPER(p_artist_name);
        INSERT INTO album_table(album_artist, album_name, album_released) VALUES(artist_id, UPPER(p_album_name), p_album_released);
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20006, 'This artist already has this album');
    END IF;
END create_album;

-------------------------CREATE_SONG (without blob)-------------------------

CREATE OR REPLACE PROCEDURE create_song
    (p_artist_name IN artist_table.artist_name%TYPE,
    p_album_name IN album_table.album_name%TYPE,
    p_song_name IN song_table.song_name%TYPE)
IS
    artist_id artist_table.artist_id%TYPE;
    album_id album_table.album_id%TYPE;
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM artist_album_song_view WHERE UPPER(album_name) = UPPER(p_album_name) AND UPPER(artist_name) = UPPER(p_artist_name) AND UPPER(song_name) = UPPER(p_song_name);
    IF (cnt = 0) THEN
        SELECT artist_id INTO artist_id FROM artist_table WHERE UPPER(artist_name) = UPPER(p_artist_name);
        SELECT album_id INTO album_id FROM album_table WHERE UPPER(album_name) = UPPER(p_album_name);
        INSERT INTO song_table(song_artist, song_album, song_name) VALUES(artist_id, album_id, UPPER(p_song_name));
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20007, 'This song + album + artist combo already exists');
    END IF;
END create_song;

-------------------------UPDATE_ARTIST-------------------------

CREATE OR REPLACE PROCEDURE update_artist
    (p_old_artist IN artist_table.artist_name%TYPE,
    p_new_artist IN artist_table.artist_name%TYPE)
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM artist_table WHERE UPPER(artist_name) = UPPER(p_old_artist);
    IF (cnt != 0) THEN
        SELECT COUNT(*) INTO cnt FROM artist_table WHERE UPPER(artist_name) = UPPER(p_new_artist);
        IF (cnt = 0) THEN
            UPDATE artist_table SET artist_name = UPPER(p_new_artist) WHERE UPPER(artist_name) = UPPER(p_old_artist);
            COMMIT;
        ELSE
            RAISE_APPLICATION_ERROR(-20005, 'Artist with this name already exists');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20008, 'Artist is not found');
    END IF;
END update_artist;

-------------------------UPDATE_ALBUM_NAME-------------------------

CREATE OR REPLACE PROCEDURE update_album_name
    (p_ablum_id IN album_table.album_id%TYPE,
    p_new_name IN album_table.album_name%TYPE)
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM album_table WHERE album_id = p_ablum_id;
    IF (cnt != 0) THEN
        UPDATE album_table SET album_name = upper(p_new_name) WHERE album_id = p_ablum_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20009, 'Album is not found');
    END IF;
END update_album_name;

-------------------------UPDATE_ALBUM_YEAR-------------------------

CREATE OR REPLACE PROCEDURE update_album_year
    (p_ablum_id IN album_table.album_id%TYPE,
    p_new_year IN album_table.album_released%TYPE)
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM album_table WHERE album_id = p_ablum_id;
    IF (cnt != 0) THEN
        UPDATE album_table SET album_released = p_new_year WHERE album_id = p_ablum_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20009, 'Album is not found');
    END IF;
END update_album_year;

-------------------------UPDATE_SONG_NAME-------------------------

CREATE OR REPLACE PROCEDURE update_song_name
    (p_song_id IN song_table.song_id%TYPE,
    p_new_name IN song_table.song_name%TYPE)
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM song_table WHERE song_id = p_song_id;
    IF (cnt != 0) THEN
        UPDATE song_table SET song_name = upper(p_new_name) WHERE song_id = p_song_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20010, 'Song is not found');
    END IF;
END update_song_name;

------------------------ insert 100 000 users

CREATE OR REPLACE PROCEDURE insert_100k_users
IS
BEGIN
    FOR i IN 1 .. 100000 LOOP
        register_user('user' || i, 'pass' || i);
    END LOOP;
END insert_100k_users;

BEGIN
insert_100k_users();
END;

commit;

------------------------------ EXPORT XML

CREATE OR REPLACE PROCEDURE song_export
IS
    rc sys_refcursor;
    doc DBMS_XMLDOM.DOMDocument;
BEGIN
    OPEN rc FOR SELECT song_id, song_name FROM song_table;
    doc := DBMS_XMLDOM.NewDOMDocument(XMLTYPE(rc));
    DBMS_XMLDOM.WRITETOFILE(doc, 'CW_DIR/song_export.xml');
END song_export;

CREATE OR REPLACE PROCEDURE users_export
IS
    rc sys_refcursor;
    doc DBMS_XMLDOM.DOMDocument;
BEGIN
    OPEN rc FOR SELECT user_id, user_login, decr, role_name FROM user_role_full_view;
    doc := DBMS_XMLDOM.NewDOMDocument(XMLTYPE(rc));
    DBMS_XMLDOM.WRITETOFILE(doc, 'CW_DIR/users_export.xml');
END users_export;

BEGIN
    users_export();
END;

---------------------- as sys to orcl:

create directory cw_dir as 'C:\CW';
select * from dba_directories where directory_name='CW_DIR';
grant read, write on directory cw_dir to dbmoonyfm;

------------------------------ IMPORT XML

CREATE OR REPLACE PROCEDURE artist_import
IS
BEGIN
    INSERT INTO artist_table (artist_name)
    SELECT ExtractValue(VALUE(artist), '//NAME') AS artist_name
    FROM TABLE(XMLSequence(EXTRACT(XMLTYPE(bfilename('CW_DIR', 'artist_import.xml'),
    nls_charset_id('UTF-8')),'/ROWSET/ROW'))) artist;
END artist_import;

BEGIN
    artist_import();
END;

delete from artist_table where artist_id in(101, 102); 
select * from artist_table;
commit;

-------------------- DELETE ARTIST

create or replace PROCEDURE delete_artist
    (p_id IN artist_table.artist_id%TYPE)
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM artist_table WHERE artist_id = p_id;
    IF (cnt != 0) THEN
        DELETE artist_table WHERE artist_id = p_id;
    ELSE
        RAISE_APPLICATION_ERROR(-20008, 'Artist is not found');
    END IF;
END delete_artist;

-------------- DELETE ALBUM

create or replace PROCEDURE delete_album
    (p_id IN album_table.album_id%TYPE)
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM album_table WHERE album_id = p_id;
    IF (cnt != 0) THEN
        DELETE artist_table WHERE artist_id = p_id;
    ELSE
        RAISE_APPLICATION_ERROR(-20009, 'Album is not found');
    END IF;
END delete_album;

------------------- DELETE SONG

create or replace PROCEDURE delete_song
    (p_id IN song_table.song_id%TYPE)
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM song_table WHERE song_id = p_id;
    IF (cnt != 0) THEN
        DELETE song_table WHERE song_id = p_id;
    ELSE
        RAISE_APPLICATION_ERROR(-20008, 'Song is not found');
    END IF;
END delete_song;

--------------- DELETE USER

create or replace PROCEDURE delete_user
    (p_login IN user_table.user_login%TYPE)
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM user_table WHERE upper(user_login) = upper(p_login);
    IF (cnt != 0) THEN
        DELETE user_table WHERE upper(user_login) = upper(p_login);
    ELSE
        RAISE_APPLICATION_ERROR(-20003, 'User is not found');
    END IF;
END delete_user;

------------------------ SAVE SONG (to playlist)

CREATE OR REPLACE PROCEDURE save_song
    (p_user_id IN saved_table.saved_user%TYPE,
    p_song_id IN saved_table.saved_song%TYPE)
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM saved_table WHERE saved_user = p_user_id AND saved_song = p_song_id;
    IF (cnt = 0) THEN
        INSERT INTO saved_table(saved_user, saved_song) VALUES(p_user_id, p_song_id);
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20011, 'This song is already saved');
    END IF;
END save_song;

------------------------- REMOVE SONG (from playlist)

CREATE OR REPLACE PROCEDURE remove_song
    (p_user_id IN saved_table.saved_user%TYPE,
    p_song_id IN saved_table.saved_song%TYPE)
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM saved_table WHERE saved_user = p_user_id AND saved_song = p_song_id;
    IF (cnt != 0) THEN
        DELETE FROM saved_table WHERE saved_user = p_user_id AND saved_song = p_song_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20012, 'This song is not on your playlist');
    END IF;
END remove_song;
