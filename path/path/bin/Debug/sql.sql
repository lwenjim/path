create table tbl_ip_domain
(
    ID INTEGER,
    IP VARCHAR(255),
    DOMAIN VARCHAR(255),
    TYPE INTEGER,
    PRIMARY KEY(ID)
);
create table tbl_ip_type
(
    ID INTEGER AUTO_INCREMENT,
    NAME VARCHAR(255),
    PRIMARY KEY(ID)
);