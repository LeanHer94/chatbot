CREATE TABLE requestsCache(
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    timezone VARCHAR(50),
    time_at_timezone DATETIME, 
    time_request DATETIME NULL); 