# Pencil-Durability-Kata

### Running Build and Tests

* Option 1: `BuildAndTest.bat` is available to run.
* Option 2: Run these commands where the solution is located:
    * `dotnet restore`
    * `dotnet build`
    * `dotnet test`
    
Note on a possible issue: Tried to run `dotnet` and command was not recognized. Had to make sure the location was in `Environment Variables` and possibly needed to run as administrator.

### Design Choice Notes
  1) Point Degradation section made no mention on how to handle punctuation or numbers. Made degrading by one the default since these should still degrade the point when written. This also prevents any unaccounted characters from falling through the cracks and being written without dulling the point.
  1) Point Degradation section didn't have an example on how to treat uppercase letters if there was still durability left but not enough to write in uppercase. Decided this would cause a space to be written and to not cause any point degredation.
  1) Eraser Degradation seemed ambiguous on how to handle whitespace. Allowed removing it if it is explicitly put into a match for erasing but only if there is still durability on the eraser. Even though it does not degrade the eraser. If there is no eraser, nothing should be allowed to be erased.
  1) Editing didn't specify how erased sections would be filled in. Decided to take in a start index to pick where to start editing and to prevent the need to save erase locations. As that would require coding how paper worked, and felt outside the scope of this project.
