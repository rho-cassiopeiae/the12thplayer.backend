SET search_path TO match_predictions;

CREATE OR REPLACE FUNCTION filter_out_already_started_fixtures (fixture_ids BIGINT[])
RETURNS TABLE ("Id" BIGINT)
LANGUAGE PLPGSQL
AS $$
BEGIN
    SET search_path TO match_predictions;

    RETURN QUERY
        SELECT f."Id"
        FROM "Fixtures" AS f
        WHERE
            f."Id" = ANY(fixture_ids) AND
            COALESCE((f."GameTime"->>'Minute')::SMALLINT, 0) = 0;
END;
$$