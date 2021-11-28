SET search_path TO identity;

CREATE OR REPLACE FUNCTION notify_about_integration_event()
    RETURNS TRIGGER
    LANGUAGE PLPGSQL
AS $$
BEGIN
    PERFORM pg_notify('integration_event_channel', NEW."Id"::text);
    RETURN NULL;
END;
$$