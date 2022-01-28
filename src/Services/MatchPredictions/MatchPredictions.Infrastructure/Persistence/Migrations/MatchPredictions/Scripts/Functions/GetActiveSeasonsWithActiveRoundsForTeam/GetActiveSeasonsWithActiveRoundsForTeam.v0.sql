SET search_path TO match_predictions;

CREATE OR REPLACE FUNCTION get_active_seasons_with_active_rounds_for_team (team_id BIGINT)
RETURNS TABLE (
    "SeasonId" BIGINT,
    "LeagueName" TEXT,
    "LeagueLogoUrl" TEXT,
    "RoundId" BIGINT,
    "RoundName" TEXT
)
LANGUAGE PLPGSQL
AS $$
BEGIN
    SET search_path TO match_predictions;

    RETURN QUERY
        SELECT s."Id", l."Name", l."LogoUrl", r."Id", r."Name"::TEXT
        FROM
            "Season" AS s
                INNER JOIN
            "Leagues" AS l
                ON (s."LeagueId" = l."Id")
                INNER JOIN
            "Rounds" AS r
                ON (s."Id" = r."SeasonId")
        WHERE
            s."Id"::TEXT IN (
                SELECT jsonb_array_elements_text("ActiveSeasons")
                FROM "TeamActiveSeasons"
                WHERE "TeamId" = team_id
            ) AND
            r."IsCurrent";
END;
$$