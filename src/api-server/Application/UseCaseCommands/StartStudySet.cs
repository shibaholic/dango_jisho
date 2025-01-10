using System.Security.Cryptography.X509Certificates;
using System.Text;
using Application.Mappings.EntityDtos.Tracking;
using MediatR;
using Application.Response;
using Application.Services;
using AutoMapper;
using Domain.Entities.Tracking;
using Domain.RepositoryInterfaces;

namespace Application.UseCaseCommands;

using Response = Response<StudySetDto>;

public class StartStudySetRequest : IRequest<Response>
{
    public Guid StudySetId { get; set; }
    public Guid UserId { get; set; }
}

public class StartStudySet : IRequestHandler<StartStudySetRequest, Response>
{
    private readonly IStudySetRepository _ssRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITimeService _time;

    public StartStudySet(IStudySetRepository ssRepo, IUnitOfWork unitOfWork, IMapper mapper, ITimeService time)
    {
        _ssRepo = ssRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _time = time;
    }

    // TODO: Currently, the day cutoff time is midnight when the date changes, but create custom user defined hour (4am for example).
    /// <summary>
    /// Users presses "start" on study set. This calculates and initializes the entry review queues for this date.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Response> Handle(StartStudySetRequest request, CancellationToken cancellationToken)
    {
        // check if userId and studySetId is valid
        var studySet = await _ssRepo.ReadByIdUserId(request.StudySetId, request.UserId);
        if (studySet == null) throw new ProblemException("Study set not found", "");
        
        // check if already started
        if (studySet.LastStartDate != null)
        {
            var now = _time.Now;
            var format = "{0, -30} {1, -30} {2, 20}" + Environment.NewLine;
            var stringBuilder = new StringBuilder().AppendFormat(format, "Date", "Date string", "Offset");
            stringBuilder.AppendFormat(format, "StudySet.LastStartDate", $"{studySet.LastStartDate.Value}");
            stringBuilder.AppendFormat(format, "_time.Now", $"{_time.Now}");
            Console.WriteLine(stringBuilder.ToString());

            if (studySet.LastStartDate.Value > now)
            {
                // Console.WriteLine("This StudySet is ahead of current time, which should never happen. But do start anyways. \n");
            }
            else if (studySet.LastStartDate.Value.Day == now.Day) // TODO: change depending on actual hour that is specified to count as a day.
            {
                // Console.WriteLine("StudySet YES started on same day. So do not re-start. \n");
                return Response.Ok("Study set not re-started.", _mapper.Map<StudySetDto>(studySet));
            }
            // Console.WriteLine($"StudySet NOT started on same day; started {nowInSameOffset.Day - studySet.LastStartDate.Value.Day} days ago. So do start. \n");
            // TODO: "restart" functionality so user can do more new words in the same day than what they had originally planned.
        }
        
        // First, query all TrackedEntries in the studySet's tags, then filter them in memory.
        var trackedEntries = await _ssRepo.GetTrackedEntriesByStudySet(request.StudySetId);
        for (int i = 0; i < trackedEntries.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {trackedEntries[i].ent_seq} {trackedEntries[i].LevelStateType}");
        }
        Console.WriteLine("");
        
        // create base queue by querying LevelStateType: Review/Know, orderByAscending: DueDate, and where DueDate passed
        var teReview = trackedEntries.Where(te => te.LevelStateType == LevelStateType.Reviewing).FirstOrDefault();
        if (teReview != null)
        {
            var lastReviewDate = teReview.LastReviewDate!.Value;
            var nextReviewDays = teReview.NextReviewDays!.Value;
            var reviewDueDate = lastReviewDate.Day + nextReviewDays;
            Console.WriteLine($"last + nextDays: {reviewDueDate}; now.day: {_time.Now.Day}");
            Console.WriteLine($"passed: {reviewDueDate <= _time.Now.Day}");
        }
        
        var baseQueue = new List<TrackedEntry>();
        baseQueue = trackedEntries.Where(te => (te.LevelStateType == LevelStateType.Known || te.LevelStateType == LevelStateType.Reviewing) && 
                                               te.LastReviewDate!.Value.AddDays((double)te.NextReviewDays!).Day <= _time.Now.Day)
            .OrderBy(te => te.LastReviewDate!.Value.AddDays((double)te.NextReviewDays!))
            .ToList();
        // TODO: smart interweaving of high score and low score entries OR should it be high nextReviewDays and low nextReviewDays?

        Console.WriteLine($"Current time: {_time.Now} baseQueue count: {baseQueue.Count}");
        foreach (var entry in baseQueue)
        {
            var entryReviewDate = entry.LastReviewDate!.Value.AddDays((double)entry.NextReviewDays!);
            string reviewDatePassed = entryReviewDate <= _time.Now ? "Passed" : "Not passed";
            Console.WriteLine($"{entry.ent_seq} {entryReviewDate} {reviewDatePassed} {entry.LevelStateType}");
        }
        Console.WriteLine();
        studySet.BaseQueue = baseQueue.Select(te => te.ent_seq).ToList();
        
        // create learning queue by querying all trackedEntries with same tags where Level = Learning and UserId
        var learningQueue = trackedEntries.Where(te => te.LevelStateType == LevelStateType.Learning)
            .OrderBy(te => te.NextReviewMinutes)
            .ToList();
        
        Console.WriteLine($"learningQueue count: {learningQueue.Count}");
        foreach (var entry in learningQueue)
        {
            Console.WriteLine($"{entry.ent_seq} {entry.NextReviewMinutes}");
        }
        Console.WriteLine();
        studySet.LearningQueue = learningQueue.Select(te => te.ent_seq).ToList();
        
        // create new queue by querying Top(NewEntryGoal) number of trackedEntries with same tags orderBy tagOrder, entryIsTagged userOrder where Level = New and UserId
        var newQueue = trackedEntries.Where(te => te.LevelStateType == LevelStateType.New)
            .Take(studySet.NewEntryGoal)
            .ToList();
        
        Console.WriteLine($"newQueue count: {newQueue.Count}");
        foreach (var entry in newQueue)
        {
            Console.WriteLine($"{entry.ent_seq} {entry.LevelStateType}");
        }
        Console.WriteLine();
        studySet.NewQueue = newQueue.Select(te => te.ent_seq).ToList();
        
        await _ssRepo.UpdateAsync(studySet);
        
        await _unitOfWork.Commit(cancellationToken);
        
        var studySetDto  = _mapper.Map<StudySetDto>(studySet);
        return Response.Ok("Study set started.", studySetDto);
    }
}