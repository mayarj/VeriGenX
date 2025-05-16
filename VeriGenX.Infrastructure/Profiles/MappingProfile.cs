using AutoMapper;
using VeriGenX.Domain.Entites;
using VeriGenX.Domain.enums;
using VeriGenX.Domain.ValueObjects;
using VeriGenX.Infrastructure.DAO;

namespace VeriGenX.Infrastructure.Profiles
{
    public class  MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<string, UserId>()
                 .ConvertUsing(src => new UserId(Guid.Parse(src)));
            CreateMap<UserDocument, User>()
                .ConvertUsing(src => User.Create(
                     new UserId(Guid.Parse(src.Id)),
                     FirstName.Create(src.FirstName).Value , 
                     LastName.Create(src.LastName).Value ,
                     Email.Create(src.Email).Value
                    ).Value );
            
            CreateMap<User, UserDocument>()
               .ForMember(des => des.Id, opt => opt.MapFrom(src => src.Id.userId.ToString()))
               .ForMember(des => des.FirstName, opt => opt.MapFrom(src => src.FirstName.Value))
               .ForMember(des => des.LastName, opt => opt.MapFrom(src => src.LastName.Value))
               .ForMember(des => des.Email, opt => opt.MapFrom(src => src.Email.Value));

            CreateMap<SignalDocument, Signal>()
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (VerilogDataType)Enum.Parse(typeof(VerilogDataType), src.Type)))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
               .ForMember(dest => dest._signalValues, opt => opt.MapFrom(src => src._signalValues));

            CreateMap<Signal, SignalDocument>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
                .ForMember(dest => dest._signalValues, opt => opt.MapFrom(src => src._signalValues));


            CreateMap<TestResultDocument, TestResult>()
               .ForMember(dest => dest.Passed, opt => opt.MapFrom(src => src.Passed))
               .ForMember(dest => dest.CodeErrorLines, opt => opt.MapFrom(src => src.CodeErrorLines))
               .ForMember(dest => dest.TestErrorLines, opt => opt.MapFrom(src => src.TestErrorLines))
               .ForMember(dest => dest.TestTime, opt => opt.MapFrom(src => src.TestTime))
               .ForMember(dest => dest.Failures, opt => opt.MapFrom(src => src.Failures));

            CreateMap<TestResult, TestResultDocument>()
                .ForMember(dest => dest.Passed, opt => opt.MapFrom(src => src.Passed))
                .ForMember(dest => dest.CodeErrorLines, opt => opt.MapFrom(src => src.CodeErrorLines))
                .ForMember(dest => dest.TestErrorLines, opt => opt.MapFrom(src => src.TestErrorLines))
                .ForMember(dest => dest.TestTime, opt => opt.MapFrom(src => src.TestTime))
                .ForMember(dest => dest.Failures, opt => opt.MapFrom(src => src.Failures));




            CreateMap<WaveformData, WaveformDataDocument>()
                .ForMember(dest => dest.TimeUnit, opt => opt.MapFrom(src => src.TimeUnit.ToString()))
                .ForMember(dest => dest.TimeScale, opt => opt.MapFrom(src => src.TimeScale))
                .ForMember(dest => dest._signals, opt => opt.MapFrom(src => src._signals));

            CreateMap<WaveformDataDocument, WaveformData>()
              .ForMember(dest => dest.TimeUnit, opt => opt.MapFrom(src => (TimeUnit)Enum.Parse(typeof(TimeUnit), src.TimeUnit)) )
              .ForMember(dest => dest.TimeScale, opt => opt.MapFrom(src => src.TimeScale))
              .ForMember(dest => dest._signals, opt => opt.MapFrom(src => src._signals));

            CreateMap<string, SnippetId>()
               .ConvertUsing(src => new SnippetId(Guid.Parse(src)));


            CreateMap<CodeSnippetDocument, CodeSnippet>()
                .ConstructUsing(src => CodeSnippet.Create(
                    new SnippetId(Guid.Parse(src.SnippetId)),
                    new ProjectId(Guid.Parse(src.ProjectId)),
                    src.Title,
                    src.Description,
                    src.VerilogCode,
                    src.TestBench).Value) 
                .ForMember(dest => dest.TestResult, opt => opt.MapFrom(src => src.TestResultDocument))
                .ForMember(dest => dest.Waveform, opt => opt.MapFrom(src => src.WaveformDataDocument))
                .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModified))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate)); 


            CreateMap<CodeSnippet, CodeSnippetDocument>()
                .ForMember(dest => dest.SnippetId, opt => opt.MapFrom(src => src.SnippetId.snippetId.ToString()))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.VerilogCode, opt => opt.MapFrom(src => src.VerilogCode))
                .ForMember(dest => dest.TestBench, opt => opt.MapFrom(src => src.TestBench))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModified))
                .ForMember(dest => dest.TestResultDocument, opt => opt.MapFrom(src => src.TestResult))
                .ForMember(dest => dest.WaveformDataDocument, opt => opt.MapFrom(src => src.Waveform))
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectId.projectId.ToString()));

            CreateMap<string, ProjectId>()
                    .ConvertUsing(src => new ProjectId(Guid.Parse(src)));

            CreateMap<ProjectDocument, Project>()
                .ConstructUsing(src => Project.Create(
                    new ProjectId(Guid.Parse(src.ProjectId)),
                    new UserId(Guid.Parse(src.UserId)),
                    src.Name,
                    src.Description).Value)
                
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModified));

            CreateMap<Project, ProjectDocument>()
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectId.projectId.ToString() ))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.userId.ToString()))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModified));
        }
    }
}
