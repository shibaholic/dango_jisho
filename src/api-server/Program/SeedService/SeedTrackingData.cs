using Application.Mappings.EntityDtos.Tracking;
using Application.Services;
using Domain.Entities;
using Domain.Entities.JMDict;
using Domain.Entities.Tracking;
using Tag = Domain.Entities.Tracking.Tag;

namespace Program.SeedService;

public static class SeedTrackingData
{
    public static async Task Run(IServiceScope serviceScope)
    {
        Console.WriteLine("Seeding with Tracking data...");

        var userCrud = serviceScope.ServiceProvider.GetService<ICrudService<User, User>>();
        var entryCrud = serviceScope.ServiceProvider.GetService<ICrudService<Entry, Entry>>();
        var tagCrud = serviceScope.ServiceProvider.GetService<ICrudService<Tag, Tag_EITDto>>();
        var eitCrud = serviceScope.ServiceProvider.GetService<ICrudService<EntryIsTagged, EntryIsTagged>>();
        var trackedCrud = serviceScope.ServiceProvider.GetService<ICrudService<TrackedEntry, TrackedEntry>>();
        var tissCrud = serviceScope.ServiceProvider.GetService<ICrudService<TagInStudySet, TagInStudySet>>();
        var studyCrud = serviceScope.ServiceProvider.GetService<ICrudService<StudySet, StudySetDto>>();

        if (userCrud == null || entryCrud == null || tagCrud == null || eitCrud == null || trackedCrud == null || tissCrud == null || studyCrud == null)
        {
            Console.WriteLine("  Error while getting CrudService<,>(s)");
            System.Environment.Exit(1);
        }

        // Get user

        var userResponse = await userCrud.GetByIdAsync(Guid.Parse("faeb2480-fbdc-4921-868b-83bd93324099"));
        if (!userResponse.Successful) throw new Exception(userResponse.Message);
        var user = userResponse.Data!;

        // Tag

        var tag1 = new Tag(user, "Mining", Guid.Parse(       "5eb7cdc6-5149-4218-9467-111111111111"));
        var tag2 = new Tag(user, "Not new words", Guid.Parse("5eb7cdc6-5149-4218-9467-222222222222"));

        // list of ent_seq

        var ent_seqs = new List<string> // length: 27
        {
            "1421540",
            // "1192700",
            // "1502790",
            // "1186760",
            // "1379430",
            // "1775870",
            // "1412640",
            // "1630050",
            // "1792970",
            // "1500100",
            // "2740680",
            // "2859672",
            // "2172220",
            // "1579310",
            // "1471130",
            // "1317830",
            // "1467550",
            // "1511500",
            // "1474050",
            // "1309550",
            // "1294540",
            // "1786080",
            // "1326600",
            // "1386350",
            // "1592380",
            // "1470020",
            // "1433840",
            // "1481670"
        };

        // Fetch entry objects

        var entries = new List<Entry>();

        foreach (var ent_seq in ent_seqs)
        {
            var response = await entryCrud.GetByIdAsync(ent_seq);
            if (response.Successful) entries.Add(response.Data!);
        }

        // Create TrackedEntry
        // don't make the mistake of creating one trackedEntry for each Entry being added to a tag.

        var trackedEntries = new List<TrackedEntry>();

        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            if (i is >= 0 and <= 5) trackedEntries.Add(new TrackedEntry(entry, user, LevelStateType.New));
            if (i is >= 6 and <= 10) trackedEntries.Add(new TrackedEntry(entry, user, LevelStateType.Learning));
            if (i is >= 11 and <= 15) trackedEntries.Add(new TrackedEntry(entry, user, LevelStateType.Reviewing));
            if (i is >= 16) trackedEntries.Add(new TrackedEntry(entry, user, LevelStateType.Known));
        }

        // Add trackedEntries to Tags
        for (var i = 0; i < trackedEntries.Count; i++)
        {
            var trackedEntry = trackedEntries[i];
            var even = i % 1 == 0;
            var success = even ? tag1.TryAddTrackedEntry(trackedEntry) : tag2.TryAddTrackedEntry(trackedEntry);
            var tagName = even ? "tag1" : "tag2";
            if (!success) Console.Error.WriteLine($"Error while adding entry {trackedEntry.ent_seq} for {tagName}.");
        }

        var tagResult = await tagCrud.CreateAsync(tag1);

        var tag2Result = await tagCrud.CreateAsync(tag2);

        // StudySet

        // var studySet = new StudySet
        // {
        //     Id = new Guid("8fac6386-976e-4eb4-bd8e-ab0f9db9270a"), UserId = tag1.UserId, LastStartDate = null,
        //     NewEntryGoal = 10, NewEntryCount = 0
        // };
        //
        // var studySetResult = await studyCrud.CreateAsync(studySet);

        // TagInStudySet

        // var tagInStudySets = new List<TagInStudySet>
        // {
        //     new TagInStudySet { TagId = tag1.Id, StudySetId = studySet.Id, Order = 1 },
        //     new TagInStudySet { TagId = tag2.Id, StudySetId = studySet.Id, Order = 2 }
        // };
        //
        // var tagInStudySet1Result = await tissCrud.CreateAsync(tagInStudySets[0]);
        // var tagInStudySet2Result = await tissCrud.CreateAsync(tagInStudySets[1]);

        // Check successful

        if (!(tagResult.Successful)) // && studySetResult.Successful )) // && tagInStudySet1Result.Successful )) // && tagInStudySet2Result.Successful
        {
            Console.WriteLine("  Error while seeding tracking data.");
            System.Environment.Exit(1);
        }

        Console.WriteLine("Successfully seeded tracking data.\n");
    }
}