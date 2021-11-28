SET search_path TO identity;

CREATE TRIGGER on_new_integration_event
AFTER INSERT
ON "IntegrationEvents"
FOR EACH ROW
EXECUTE PROCEDURE notify_about_integration_event();