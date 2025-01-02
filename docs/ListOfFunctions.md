# List of Functions

## Query Functions

Functions that don't change data and simply query, but still require their own controller and handler.

| s/n | Name         | Details                             | Associated client view | Status |
|-----|--------------|-------------------------------------|------------------------|--------|
| 1   | Entry search | Searches entries by a search string | Landing                | ✅      |

## Modifying functions

Each of these functions is an API endpoint and associated handlers that create, update or delete server persistant data.

This list compiles all of these functions in one neat place.

| s/n | Name               | Details                                                 | Associated client view                           | Status |
|-----|--------------------|---------------------------------------------------------|--------------------------------------------------|--------|
| 1   | Add To Tag         | Associates an entry to a user defined tag               | List of words query, flashcard back, word detail | ✅      |
| 2   | Add To Group       | Associates an entry to a user defined group             | List of words query, flashcard back, word detail | ❌      |
| 3   | Change Entry Order | Change the user defined order of entries in a tag/group | List of words query, flashcard back, word detail | ❌      |
| 3   | Review Entry       | Appends a tracked entry with a review event             | Flashcard back                                   | ❌      |
| 4   | Edit Entry Note    | Adds/Updates the entry's user defined note              | Flashcard back, word detail                      | ❌      |
| 5   | Edit Entry Sense   | Updates the entry's user defined sense                  | Flashcard back, word detail                      | ❌      |
| 6   |                    |                                                         |                                                  |        |
| 7   |                    |                                                         |                                                  |        |
| 8   |                    |                                                         |                                                  |        |
| 9   |                    |                                                         |                                                  |        |
| 10  |                    |                                                         |                                                  |        |