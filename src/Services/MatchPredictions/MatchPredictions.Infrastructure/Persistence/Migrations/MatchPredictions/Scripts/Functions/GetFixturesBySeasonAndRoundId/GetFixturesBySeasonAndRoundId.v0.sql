SET search_path TO match_predictions;

CREATE OR REPLACE FUNCTION get_fixtures_by_season_and_round_id (
    season_ids BIGINT[],
    round_ids BIGINT[]
)
RETURNS TABLE (
    "Id" BIGINT,
    "SeasonId" BIGINT,
    "RoundId" BIGINT,
    "StartTime" BIGINT,
    "Status" TEXT,
    "GameTime" JSONB,
    "Score" JSONB,
    "HomeTeamName" TEXT,
    "HomeTeamLogoUrl" TEXT,
    "GuestTeamName" TEXT,
    "GuestTeamLogoUrl" TEXT
)
LANGUAGE PLPGSQL
AS $$
BEGIN
    SET search_path TO match_predictions;

    RETURN QUERY
        SELECT
            f."Id", f."SeasonId", f."RoundId", f."StartTime", f."Status", f."GameTime", f."Score",
            ht."Name", ht."LogoUrl", gt."Name", gt."LogoUrl"
        FROM
            "Fixtures" AS f
                INNER JOIN
            "Teams" AS ht
                ON (f."HomeTeamId" = ht."Id")
                INNER JOIN
            "Teams" AS gt
                ON (f."GuestTeamId" = gt."Id")
        WHERE (f."SeasonId", f."RoundId") IN (
            SELECT *
            FROM UNNEST (season_ids, round_ids)
        );
END;
$$