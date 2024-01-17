using Application.Activities;
using Application.Comments;
using Application.Profiles;
using AutoMapper;
using Domain;
using Microsoft.Extensions.Options;

namespace Application.Core
{
  public class MappingProfiles : AutoMapper.Profile
  {
    public MappingProfiles()
    {
      string currentUsername = null;
      CreateMap<Activity, Activity>();
      CreateMap<Activity, ActivityDto>()
        .ForMember(d => d.HostUsername, options => options
        .MapFrom(s => s.Attendees
        .FirstOrDefault(x => x.IsHost).AppUser.UserName));

      CreateMap<ActivityAttendee, AttendeeDto>()
        .ForMember(d => d.DisplayName, options => options.MapFrom(s => s.AppUser.DisplayName))
        .ForMember(d => d.Username, options => options.MapFrom(s => s.AppUser.UserName))
        .ForMember(d => d.Bio, options => options.MapFrom(s => s.AppUser.Bio))
        .ForMember(d => d.Image, options => options.MapFrom(s => s.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url))
        .ForMember(d => d.FollowersCount, options => options.MapFrom(s => s.AppUser.Followers.Count))
        .ForMember(d => d.FollowingCount, options => options.MapFrom(s => s.AppUser.Followings.Count))
        .ForMember(d => d.Following, options => options.MapFrom(s => s.AppUser.Followers.Any(x => x.Observer.UserName == currentUsername)));

      CreateMap<AppUser, Profiles.Profile>()
        .ForMember(d => d.Image, options => options.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain).Url))
        .ForMember(d => d.FollowersCount, options => options.MapFrom(s => s.Followers.Count))
        .ForMember(d => d.FollowingCount, options => options.MapFrom(s => s.Followings.Count))
        .ForMember(d => d.Following, options => options.MapFrom(s => s.Followers.Any(x => x.Observer.UserName == currentUsername)));

      CreateMap<Comment, CommentDto>()
        .ForMember(d => d.DisplayName, options => options.MapFrom(s => s.Author.DisplayName))
        .ForMember(d => d.Username, options => options.MapFrom(s => s.Author.UserName))
        .ForMember(d => d.Image, options => options.MapFrom(s => s.Author.Photos.FirstOrDefault(x => x.IsMain).Url));

      CreateMap<ActivityAttendee, UserActivityDto>()
      .ForMember(d => d.ActivityId, options => options.MapFrom(s => s.Activity.Id))
      .ForMember(d => d.Date, options => options.MapFrom(s => s.Activity.Date))
      .ForMember(d => d.Title, options => options.MapFrom(s => s.Activity.Title))
      .ForMember(d => d.Category, options => options.MapFrom(s => s.Activity.Category))
      .ForMember(d => d.HostUsername, options => options.MapFrom(s => s.Activity.Attendees.FirstOrDefault(x => x.IsHost).AppUser.UserName));
    }
  }
}