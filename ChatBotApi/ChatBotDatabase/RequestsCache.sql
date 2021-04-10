CREATE TABLE requestsCache(
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    timezone VARCHAR(20),
    time_at_timezone DATETIME, 
    time_request DATETIME NULL); 