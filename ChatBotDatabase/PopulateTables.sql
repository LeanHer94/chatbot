INSERT INTO zones (id,[zone],parent,available) VALUES (1,'America',NULL,0);
INSERT INTO zones (id,[zone],parent,available) VALUES (2,'Argentina','America',0);
INSERT INTO zones (id,[zone],parent,available) VALUES (3,'Buenos Aires','Argentina',1);
INSERT INTO zones (id,[zone],parent,available) VALUES (4,'Catamarca','Argentina',1);
INSERT INTO zones (id,[zone],parent,available) VALUES (5,'Bogota','America',1);
INSERT INTO zones (id,[zone],parent,available) VALUES (6,'Indiana','America',0);
INSERT INTO zones (id,[zone],parent,available) VALUES (7,'Marengo','Indiana',1);
INSERT INTO zones (id,[zone],parent,available) VALUES (8,'Jamaica','Indiana',1);

INSERT INTO requests (user_id,zone_id) VALUES (1,3);
INSERT INTO requests (user_id,zone_id) VALUES (1,3);
INSERT INTO requests (user_id,zone_id) VALUES (1,4);
INSERT INTO requests (user_id,zone_id) VALUES (1,5);
INSERT INTO requests (user_id,zone_id) VALUES (1,7);
INSERT INTO requests (user_id,zone_id) VALUES (1,7);
INSERT INTO requests (user_id,zone_id) VALUES (1,8);