using AutoMapper;
using AutoMapper.QueryableExtensions;
using JobBoard.Data.Repository;
using JobBoard.Data.UnitOfWork;
using JobBoard.Domain.Entities;
using JobBoard.Dtos;
using JobBoard.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobBoard.Services
{
    public class JobOfferService : IJobOfferService
    {
        public IUnitOfWork Uow { get; set; }
        public IMapper Mapper { get; set; }

        private IRepository<JobOffer> Repo => Uow.Repository<JobOffer>();

        public IEnumerable<JobOfferListDto> List(bool onlyActive = true)
        {
            var q = Repo.Query();
            if (onlyActive) q = q.Where(x => x.IsActive);
            return q.OrderByDescending(x => x.PublishedAt)
                    .ProjectTo<JobOfferListDto>(Mapper.ConfigurationProvider)
                    .ToList();
        }

        public JobOfferListDto Get(int id)
        {
            var e = Repo.Find(id);
            return e == null ? null : Mapper.Map<JobOfferListDto>(e);
        }

        public JobOfferListDto Create(JobOfferCreateDto dto)
        {
            var e = Mapper.Map<JobOffer>(dto);
            if (e.PublishedAt == default) e.PublishedAt = DateTime.UtcNow;
            Repo.Add(e);
            Uow.SaveChanges();
            return Mapper.Map<JobOfferListDto>(e);
        }

        public void Update(int id, JobOfferUpdateDto dto)
        {
            var e = Repo.Find(id);
            if (e == null) return;
            if (e.Id != dto.Id) return;
            var u = Mapper.Map<JobOfferUpdateDto, JobOffer>(dto, e);
            u.UpdatedAt = DateTime.UtcNow;
            Repo.Update(u);
            Uow.SaveChanges();
        }

        public void Delete(int id)
        {
            var e = Repo.Find(id);
            if (e == null) return;
            Repo.Remove(e);
            Uow.SaveChanges();
        }

        public void Apply(int offerId, string name, string email)
        {
            System.Diagnostics.Debug.WriteLine($"APPLY -> OfferId:{offerId} Name:{name} Email:{email}");
        }
    }
}
