SET search_path TO livescore;

CREATE OR REPLACE FUNCTION get_players_with_country_from (team_id BIGINT)
RETURNS table (
    "Id" BIGINT,
    "FirstName" TEXT,
    "LastName" TEXT,
    "BirthDate" BIGINT,
    "CountryName" TEXT,
    "CountryFlagUrl" TEXT,
    "Number" SMALLINT,
    "Position" TEXT,
    "ImageUrl" TEXT
)
LANGUAGE PLPGSQL
AS $$
BEGIN
    SET search_path TO livescore;

    RETURN QUERY
        SELECT
            p."Id", p."FirstName", p."LastName", p."BirthDate", c."Name",
            c."FlagUrl", p."Number", p."Position", p."ImageUrl"
        FROM
            "Players" AS p
                LEFT JOIN
            "Countries" AS c
                on (p."CountryId" = c."Id")
        WHERE p."TeamId" = team_id AND p."Number" IS NOT NULL;
END;
$$