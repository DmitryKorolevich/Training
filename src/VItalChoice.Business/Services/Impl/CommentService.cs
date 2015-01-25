using System;
using System.Collections.Generic;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Infrastructure;
using VitalChoice.Business.Queries.Comment;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Infrastructure.UnitOfWork;

namespace VitalChoice.Business.Services.Impl
{
	public class CommentService: GenericService<Comment>, ICommentService
	{
		public CommentService(IRepositoryAsync<Comment> repository) : base(repository)
		{
		}

		public IEnumerable<Comment> QueryByText(string text)
		{
			var query = new CommentQuery();

			return Repository.Query(query.CommentStartsWith(text)).Select();
		}

		public void InsertWithUser(Comment comment)
		{
			//comment.Id = (new Random()).Next(1, 10000000);
			comment.CreationDate = DateTime.Now;
			comment.Text = "atatatatatat";
			//comment.ObjectState = ObjectState.Added;
			comment.AuthorId = "21496c6d-5239-417a-a737-5b3cd7b97a3d";

			Repository.Insert(comment);

			using (var uof = new VitalChoiceUnitOfWork())
			{
				uof.BeginTransaction();

				var uofRepo = uof.RepositoryAsync<Comment>();

				comment = new Comment();
				comment.CreationDate = DateTime.Now;
				comment.Text = "123123123";
				//comment.ObjectState = ObjectState.Added;
				comment.AuthorId = "21496c6d-5239-417a-a737-5b3cd7b97a3d";
				uofRepo.Insert(comment);

				uof.SaveChanges();

				uof.Commit();
            }
		}
	}
}
