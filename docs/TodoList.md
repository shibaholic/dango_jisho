# Todo List

- Query string cleaning (white space)

## Dec 12 2024

1. ✅ Backend review system
   - TrackedEntry
     - Creation, deletion
     - Status state machine
   - ReviewEvent
2. ✅ Backend tag system
3. Backend user JWT configuration
   - Integration testing!!!!!
4. ✅ Switch to packaging the JMdict data with the application
   - Migration would be done manually by the Admin anyways for the most part.
   - Makes testing and seeding data easier and faster.
5. ✅ Switch to PostgreSQL copy for JMdict repository instead of AddRange
6. Review Score algorithm

## Feb 11 2025

1. Create a key to sort Entries by relevance, then you don't need to Order By in the search query which prevents issues with pagination and taking... Or maybe not... 
