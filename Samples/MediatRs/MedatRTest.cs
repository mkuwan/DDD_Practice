using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using MediatR;
using Moq;
using Xunit;

namespace Samples.MediatRs
{
    public class MedatRTest
    {
        [Fact]
        public async void MediatRGetModelListTest()
        {
            var mediatorMock = new Mock<IMediator>();
            List<Model> models = new List<Model>();
            for (int i = 0; i < 5; i++)
            {
                models.Add(new Model(i, i + "さんです"));
            }

            mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<List<Model>>>(), default))
                .ReturnsAsync(models)
                .Verifiable();
            var queryAll = new GetAllQuery();


            var results = await mediatorMock.Object.Send(queryAll);

            Assert.Equal(5, results.Count);
            mediatorMock.Verify(x => x.Send(It.IsAny<IRequest<List<Model>>>(), default), Times.Once);
        }

        [Fact]
        public async void MediatRGetModelTest()
        {
            var mediatorMock = new Mock<IMediator>();
            List<Model> models = new List<Model>();
            for (int i = 0; i < 5; i++)
            {
                models.Add(new Model(i, i + "さんです"));
            }
            mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<Model?>>(), default))
                .ReturnsAsync(models.Find(x => x.Id == 1))
                .Verifiable();
            var queryOne = new GetOneQuery(1);


            var result = await mediatorMock.Object.Send(queryOne);

            Assert.Equal("1さんです", result.Name);
            mediatorMock.Verify(x => x.Send(It.IsAny<IRequest<Model?>>(), default), Times.Once);
        }
    }

    internal class GetAllQuery : IRequest<List<Model>> { }
    internal class GetAllQueryHandler : IRequestHandler<GetAllQuery, List<Model>>
    {
        private readonly ModelService service;

        public GetAllQueryHandler()
        {
            this.service = new ModelService();
        }

        public async Task<List<Model>> Handle(GetAllQuery request, CancellationToken cancellationToken)
        {
            return await service.GetModels();
        }
    }

    internal class GetOneQuery : IRequest<Model>
    {
        public GetOneQuery(int id)
        {
            this.id = id;
        }

        public int id { get; }
    }
    internal class GetOneQueryHandler : IRequestHandler<GetOneQuery, Model>
    {
        private readonly ModelService service;
        
        public GetOneQueryHandler(ModelService service)
        {
            this.service = service;
        }

        public async Task<Model> Handle(GetOneQuery request, CancellationToken cancellationToken)
        {
            return await service.GetModel(request.id);
        }
    }

    internal class ModelService
    {
        List<Model> models = new List<Model>();

        public async Task<List<Model>> GetModels()
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    models.Add(new Model(i, i + "さんです"));
                }
            });
            
            return models;
        }

        public async Task<Model?> GetModel(int id)
        {
            var _models = models ?? await GetModels();

            return _models.Find(x => x.Id == id);
        }
    }

    internal class Model
    {
        public Model(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
