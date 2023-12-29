using AutoMapper;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Common;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.Models;

namespace WebAPI.Net6.BaseRepositoryGenerics.Extensions
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            this.CreateMap<TErrorMessage, ErrorMessageViewModel>();
        }
    }
}
