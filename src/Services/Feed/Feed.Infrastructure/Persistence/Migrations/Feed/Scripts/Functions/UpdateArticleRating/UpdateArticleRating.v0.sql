SET search_path TO feed;

CREATE OR REPLACE FUNCTION update_article_rating (
	article_id INT,
	increment_rating_by INT,
	OUT updated_rating INT
)
LANGUAGE PLPGSQL
AS $$
BEGIN
	SET search_path TO feed;

	UPDATE "Articles"
	SET "Rating" = "Rating" + increment_rating_by
	WHERE "Id" = article_id
	RETURNING "Rating"
	INTO updated_rating;
END;
$$