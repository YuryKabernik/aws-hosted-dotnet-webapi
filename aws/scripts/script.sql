CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240914143535_Initial') THEN
    CREATE TABLE IF NOT EXISTS "Images" (
        "Name" text NOT NULL,
        "Size" bigint NOT NULL,
        "Extension" text NOT NULL,
        "LastUpdate" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Images" PRIMARY KEY ("Name")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240914143535_Initial') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20240914143535_Initial', '8.0.8');
    END IF;
END $EF$;
COMMIT;

