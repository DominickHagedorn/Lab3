namespace Lab3;

public enum AirportInputError
{
    InvalidIDLength,
    DuplicateAirportId,
    InvalidCityLength,
    InvalidTitleLength,
    RatingOutOfBounds,
    InvalidDate,
    DBAdditionError,
    NoError
}

public enum AirportAddError
{
    AirportAlreadyExists,
    NoAirportPassed,
    NoError,
    DatabaseError
}
public enum AirportDeletionError
{
    AirportNotFound,
    FailedToDeleteError,
    NoError
}

public enum AirportEditError
{
    DBAdditionError,
    InvalidIDLength,
    DuplicateAirportId,
    InvalidCityLength,
    InvalidTitleLength,
    RatingOutOfBounds,
    InvalidDate,
    IdNotPresent,
    DatabaseError,
    Error,
    NoError
}
