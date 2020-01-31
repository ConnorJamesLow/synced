### v0.3.0
Unique Keys Update
 * `Synchronize.CreateTable` now uses the `[Unique]` attribute to specify a unique key column in the generated SQL.

### v0.2.0
Primary Keys Update
 * `Synchronize.CreateTable` now uses the `[Identity]` attribute to specify a primary key column in the generated SQL.
 * Added `Synchronize.SyncTable(Type type)` overload.

### v0.1.1
Patches
 * Fixed bugs with ColumnSize not being applied to created tables.

### v0.1.0
Initial release
 * **Synced.Synchronize** class
    * `SyncTable<T>()`
    * `CreateTable(Type type)`
    * `DropTable(string tableName)`
    * `DeleteRecords(string tableName)`
 * **Synced.Attributes** namespace
    * AllowNull
    * DataSize
    * DataType
    * Identity
    * Unique