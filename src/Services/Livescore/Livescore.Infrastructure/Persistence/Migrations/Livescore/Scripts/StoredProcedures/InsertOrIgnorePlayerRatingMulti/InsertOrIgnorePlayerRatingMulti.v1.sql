SET search_path TO livescore;

CREATE OR REPLACE PROCEDURE insert_or_ignore_player_rating_multi (
	fixture_id bigint,
	team_id bigint,
	participant_keys text[],
	total_ratings int[],
	total_voters int[]
)
LANGUAGE PLPGSQL
AS $$
BEGIN
	SET search_path TO livescore;

	INSERT INTO "PlayerRatings" ("FixtureId", "TeamId", "ParticipantKey", "TotalRating", "TotalVoters")
		SELECT fixture_id, team_id, vals.pk, vals.tr, vals.tv
		FROM UNNEST (participant_keys, total_ratings, total_voters) AS vals (pk, tr, tv)
	ON CONFLICT DO NOTHING;
END;
$$