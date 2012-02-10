DynamicLinq
===========

A library for simple reads and writes to a SQL database.  No configuration is necessary other than specifying a dialect and how to connect to the database:

    DB db = new DB(new SQLiteDialect(), () => new SQLiteConnection("Data source:memory:"));

Inserting into a table:

    db.Insert(new {FirstName = "John", LastName = "Smith"})
      .Into(x => x.People);

Updating record(s):

    db.Update(x => x.People)
      .Set(new {FirstName = "Bob"})
      .Where(record => record.Id == 56)
      .Execute();

Querying:

    dynamic people = db.Query(x => x.People)
                       .Where(record => record.FirstName != "John")
                       .Select(record => record.FirstName + " " + record.LastName);

### SQL Support

* Inserts
* Updates
* Queries
    * Where clauses, including calculations
    * Single joins
    * Select statements, including calculated columns
    * Skip/Take
    * OrderBy
    * Count