# Synced
###### v0.3.0
A simple .NET library for building SQL tables from C# classes.  

## Features

#### Synced.Synchronize
Provides the core functionality of the Synced library. Basic usage:  
```C#
// Initialize the Synchronize instance with a connection string.  
var synchronizer = new Synchronize(connectionString);

// Create a table, or replace an exsiting one, using a class as a template.
synchronizer.SyncTable<MyClass>();

// Optionally, tell the method to force dropping a table, if it already has records.
synchronizer.SyncTable<AnotherClass>(Synced.SyncFlags.ForceDrop);
```

The `Synchronize` class also exposes the logical components of the `SyncTable` method:  
```C#
Type table = typeof(MyClass);
string tableName = table.Name;

// Check to see if the INFORMATION_SCHEMA.TABLES contains a table with the given name.
if (synchronizer.TableExists(tableName)) {

  // Check to see if dao.MyTable has any records.
  if (synchronizer.TableHasRows(tableName)) {

    // DELETE FROM MyClass
    synchronizer.DeleteRecords(tableName);
  }

  // DROP TABLE MyClass
  synchronizer.DropTable(tableName);
}

// CREATE TABLE MyClass
synchronizer.CreateTable(tableName);
```

#### Synced.Attributes namespace
Synced also provides attributes to apply to your class properties which tell it how to structure the table columns:
 * `AllowNulls`: `NULL` instead of default `NOT NULL`.
 * `DataType(type: ColumnType, [scale: int, [precision: int]])`: Override the default binding for a C# type to a SQL type. Use the `ColumnType` enum to assign. **Example:** `[DataType(ColumnType.nvarchar, 123)]` becomes `nvarchar(123)`.
 * `DataSize(size: int | scale: int, precision: int)`: Targets the size portion of the DataType (i.e. the last two parameters of `DataType`).
 * `Identity([seed: int, increment: int])`: Sets the identity of a table. Optionally set the seed and increment.

## TODO 
Some ideas for next features:  


 - [ ] Add `TryUpdate` ability, i.e. if a table exists and has rows, determine whether it would be possible to update the current records to match the new schema.
 - [x] Add Primary Keys.
 - [ ] Add Unique Keys.
 - [ ] Check if a table update is even necessary (i.e. if the structure hasn't changed).
 - [ ] Add a `SyncCollection` method that uses a class as a schema for the Database, with properties as tables.
 - [ ] Use Native attributes as alternatives to Synced.Attributes, e.g. `System.ComponentModel.DataAnnotations`.