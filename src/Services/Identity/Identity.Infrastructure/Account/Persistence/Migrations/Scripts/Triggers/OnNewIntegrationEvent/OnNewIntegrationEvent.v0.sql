SET search_path TO identity;

CREATE OR REPLACE FUNCTION notify_about_integration_event()
    RETURNS TRIGGER
    LANGUAGE PLPGSQL
AS $$
BEGIN
    PERFORM pg_notify('integration_event_channel', row_to_json(NEW)::text);
    RETURN NULL;
END;
$$;

CREATE TRIGGER on_new_integration_event
AFTER INSERT
ON "IntegrationEvents"
FOR EACH ROW
EXECUTE PROCEDURE notify_about_integration_event();