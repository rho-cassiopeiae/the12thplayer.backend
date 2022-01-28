SET search_path TO match_predictions;

CREATE OR REPLACE FUNCTION get_user_predictions_by_season_and_round_id (
    user_id BIGINT,
    season_ids BIGINT[],
    round_ids BIGINT[]
)
RETURNS TABLE (
    "UserId" BIGINT,
    "SeasonId" BIGINT,
    "RoundId" BIGINT,
    "FixtureIdToScore" JSONB
)
LANGUAGE PLPGSQL
AS $$
BEGIN
    SET search_path TO match_predictions;

    RETURN QUERY
        SELECT up."UserId", up."SeasonId", up."RoundId", up."FixtureIdToScore"
        FROM "UserPredictions" AS up -- @@NOTE: Have to use an alias to avoid conflicts between columns and variables.
        WHERE
            up."UserId" = user_id AND
            (up."SeasonId", up."RoundId") IN (
                SELECT *
                FROM UNNEST (season_ids, round_ids)
            );
END;
$$