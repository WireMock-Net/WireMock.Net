
using System;
using FluentBuilder;
using WireMock.Admin.Mappings;

namespace FluentBuilder
{
    public partial class BodyModelBuilder2 : Builder<BodyModel>
    {

        private Lazy<WireMock.Admin.Mappings.MatcherModel> _matcher = new Lazy<WireMock.Admin.Mappings.MatcherModel>(() => default(WireMock.Admin.Mappings.MatcherModel));

        public BodyModelBuilder WithMatcher(WireMock.Admin.Mappings.MatcherModel value) => WithMatcher(() => value);

        public BodyModelBuilder WithMatcher(Func<WireMock.Admin.Mappings.MatcherModel> func)
        {
            _matcher = new Lazy<WireMock.Admin.Mappings.MatcherModel>(func);

            return this;
        }

        public BodyModelBuilder WithoutMatcher() => WithMatcher(() => default(WireMock.Admin.Mappings.MatcherModel));

        private Lazy<WireMock.Admin.Mappings.MatcherModel[]> _matchers = new Lazy<WireMock.Admin.Mappings.MatcherModel[]>(default(WireMock.Admin.Mappings.MatcherModel[]));

        public BodyModelBuilder WithMatchers(WireMock.Admin.Mappings.MatcherModel[] value) => WithMatchers(() => value);

        public BodyModelBuilder WithMatchers(Func<WireMock.Admin.Mappings.MatcherModel[]> func)
        {
            _matchers = new Lazy<WireMock.Admin.Mappings.MatcherModel[]>(func);

            return this;
        }

        public BodyModelBuilder WithoutMatchers() => WithMatchers(() => default(WireMock.Admin.Mappings.MatcherModel[]));

        public override BodyModel Build()
        {
            if (Object?.IsValueCreated != true)
            {
                Object = new Lazy<BodyModel>(new BodyModel
                {
                    Matcher = _matcher.Value,
                    Matchers = _matchers.Value,

                });
            }

            PostBuild(Object.Value);

            return Object.Value;
        }

        public static BodyModel Default() => new BodyModel();

    }
}