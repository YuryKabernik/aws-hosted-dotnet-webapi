namespace DataConsistencyFunction.Commands;

public static class SqlCommands
{
    public static readonly string CountRows = "SELECT COUNT(*) FROM Images;";

    public static readonly string CreateIfNotExists =
        """
        CREATE TABLE IF NOT EXISTS Images (
            "Name" text NOT NULL,
            "Size" bigint NOT NULL,
            "Extension" text NOT NULL,
            "LastUpdate" timestamp with time zone NOT NULL,
            CONSTRAINT "PK_Images" PRIMARY KEY ("Name")
        )
        """;
}