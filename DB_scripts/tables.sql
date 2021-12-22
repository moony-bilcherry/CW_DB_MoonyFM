ALTER SESSION SET "_ORACLE_SCRIPT" = TRUE;

SELECT * FROM user_tables;

-------------------------ROLE-------------------------

CREATE TABLE role_table (
    role_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    role_name VARCHAR2(30) NOT NULL,
    CONSTRAINT role_pk PRIMARY KEY (role_id)
);

INSERT INTO role_table(role_name) VALUES('USER');
INSERT INTO role_table(role_name) VALUES('OWNER');

SELECT * FROM role_table;
DROP TABLE role_table;



-------------------------USER-------------------------

CREATE TABLE user_table (
    user_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    user_login VARCHAR2(30) NOT NULL,
    user_password VARCHAR2(200) NOT NULL,
    user_role NUMBER(10) NOT NULL,
    CONSTRAINT user_pk PRIMARY KEY (user_id),
    CONSTRAINT user_role_fk FOREIGN KEY (user_role) REFERENCES role_table(role_id)
);

SELECT * FROM user_table;
select count(*) from user_table;
DROP TABLE user_table;

-------------------------ARTIST-------------------------

CREATE TABLE artist_table (
    artist_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    artist_name VARCHAR2(30) NOT NULL,
    CONSTRAINT artist_pk PRIMARY KEY (artist_id)
);

INSERT INTO artist_table(artist_name) VALUES('Polyphia');
INSERT INTO artist_table(artist_name) VALUES('Radiohead');
INSERT INTO artist_table(artist_name) VALUES('Arctic Monkeys');
INSERT INTO artist_table(artist_name) VALUES('Pink Floyd');
INSERT INTO artist_table(artist_name) VALUES('Placebo');

SELECT * FROM artist_table;
DROP TABLE artist_table;

-------------------------ALBUM-------------------------

CREATE TABLE album_table (
    album_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    album_artist NUMBER(10) NOT NULL,
    album_name VARCHAR2(30) NOT NULL,
    album_released NUMBER(10) NOT NULL,
    album_blob BLOB DEFAULT EMPTY_BLOB(),
    CONSTRAINT album_pk PRIMARY KEY (album_id),
    CONSTRAINT album_artist_fk FOREIGN KEY (album_artist) REFERENCES artist_table(artist_id)
);

alter table album_table add constraint fk_artist_album foreign key (album_artist) references artist_table (artist_id) on delete cascade;

INSERT INTO album_table(album_artist, album_name, album_released) VALUES(1, 'New Levels New Devils', 2018);
INSERT INTO album_table(album_artist, album_name, album_released) VALUES(2, 'OK Computer', 1997);
INSERT INTO album_table(album_artist, album_name, album_released) VALUES(2, 'In Rainbows', 2007);
INSERT INTO album_table(album_artist, album_name, album_released) VALUES(2, 'Kid A', 2000);
INSERT INTO album_table(album_artist, album_name, album_released) VALUES(3, 'AM', 2013);
INSERT INTO album_table(album_artist, album_name, album_released) VALUES(3, 'Favourite Worst Nightmare', 2007);
INSERT INTO album_table(album_artist, album_name, album_released) VALUES(4, 'The Wall', 1979);
INSERT INTO album_table(album_artist, album_name, album_released) VALUES(4, 'Wish You Were Here', 1975);
INSERT INTO album_table(album_artist, album_name, album_released) VALUES(5, 'Loud Like Love', 2013);
INSERT INTO album_table(album_artist, album_name, album_released) VALUES(5, 'Meds', 2013);

SELECT * FROM album_table;

delete from album_table;
commit;
DROP TABLE album_table;

-------------------------SONG-------------------------

CREATE TABLE song_table (
    song_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    song_artist NUMBER(10) NOT NULL,
    song_album NUMBER(10) NOT NULL,
    song_name VARCHAR2(30) NOT NULL,
    song_blob BLOB DEFAULT EMPTY_BLOB(),
    CONSTRAINT song_pk PRIMARY KEY (song_id),
    CONSTRAINT song_artist_fk FOREIGN KEY (song_artist) REFERENCES artist_table(artist_id),
    CONSTRAINT song_album_fk FOREIGN KEY (song_album) REFERENCES album_table(album_id)
);

SELECT * FROM song_table;
delete from song_table;
commit;
DROP TABLE song_table;

------------------------ SAVED

CREATE TABLE saved_table (
    saved_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    saved_user NUMBER(10) NOT NULL,
    saved_song NUMBER(10) NOT NULL,
    CONSTRAINT saved_pk PRIMARY KEY (saved_id),
    CONSTRAINT saved_user_fk FOREIGN KEY (saved_user) REFERENCES user_table(user_id),
    CONSTRAINT saved_song_fk FOREIGN KEY (saved_song) REFERENCES song_table(song_id)
);

insert into saved_table(saved_user, saved_song) values (2,41);
SELECT * FROM saved_table;
delete from saved_table;
commit;
DROP TABLE saved_table;
