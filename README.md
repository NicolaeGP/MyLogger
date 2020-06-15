# MyLogger
The given LogComponent was a small library with too many bugs and overly complicated.\
Issues at first glance:
- Everything is hard-coded
- Class attributes declared all over the code
- One single loop that did everything
- A random f variable compared to a random number 5
- Naming issues
- The use of Thread instead of Task (no point in keeping a thread occupied while the logger awaits the delay)
- No way to test the midnight creation of new log file
- Project using .NET Framework

Due to these issues and the fact that the library is small I have decided it will be faster and easier to start fresh with a .net standard library project

Besides fixing the issues stated above I have added the posibility of settting a custom log formatter and setting the logs folder path.
I have also tried not overengineering this since I did not want to end up with this: https://github.com/raelyard/Overengineered-Hello-World

## Room for improvement

- The writer should be injected in in logger
  - Create IWriter interface
  - Create FileWriter that takes in a folder path and has public method to write and private method to handle change date (use datetime provider for testing)
  - Inject IWriter instead of log folder path in the logger constructor
  - File writer should be a singleton (you can't have different loggers to different folder paths) or we must create a factory to keep track of existing writers and if you request one for a specific folder, and one for that folder exists it must return the same one and not create a new instance (otherwise it's going to crash with "file is opened in another program"; assuming we use the same naming scheme for all)
- The logger should be able to pick up the configurations from a config file
- Add option to select the naming scheme for the log files
- Much more testing 
- Better recovery handling.

¯\\\_(ツ)_/¯