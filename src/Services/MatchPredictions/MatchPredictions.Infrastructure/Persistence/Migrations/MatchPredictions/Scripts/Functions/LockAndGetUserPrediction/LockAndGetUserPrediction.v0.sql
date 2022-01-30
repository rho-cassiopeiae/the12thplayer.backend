SET search_path TO match_predictions;

CREATE OR REPLACE FUNCTION lock_and_get_user_prediction (
    user_id BIGINT,
    season_id BIGINT,
    round_id BIGINT
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

    PERFORM pg_advisory_xact_lock(hashtextextended(CONCAT(user_id, ' ', season_id, ' ', round_id), 0));

    RETURN QUERY
        SELECT up."UserId", up."SeasonId", up."RoundId", up."FixtureIdToScore"
        FROM "UserPredictions" AS up
        WHERE
            up."UserId" = user_id AND
            up."SeasonId" = season_id AND
            up."RoundId" = round_id;
END;
$$