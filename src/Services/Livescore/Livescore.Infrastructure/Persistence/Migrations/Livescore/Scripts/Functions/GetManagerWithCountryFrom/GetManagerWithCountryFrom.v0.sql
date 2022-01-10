SET search_path TO livescore;

CREATE OR REPLACE FUNCTION get_manager_with_country_from (team_id BIGINT)
RETURNS table (
    "Id" BIGINT,
    "FirstName" TEXT,
    "LastName" TEXT,
    "BirthDate" BIGINT,
    "CountryName" TEXT,
    "CountryFlagUrl" TEXT,
    "ImageUrl" TEXT
)
LANGUAGE PLPGSQL
AS $$
BEGIN
    SET search_path TO livescore;

    RETURN QUERY
        SELECT m."Id", m."FirstName", m."LastName", m."BirthDate", c."Name", c."FlagUrl", m."ImageUrl"
        FROM
            "Managers" AS m
                LEFT JOIN
            "Countries" AS c
                on (m."CountryId" = c."Id")
        WHERE m."TeamId" = team_id;
END;
$$