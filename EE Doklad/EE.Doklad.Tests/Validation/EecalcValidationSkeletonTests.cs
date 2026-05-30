using Xunit;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcValidationSkeletonTests
    {
        [Fact(Skip = "Validation fixtures and EECalc expected snapshots are not implemented yet.")]
        public void Compare_EecalcExpected_To_EeDokladActual_Skeleton()
        {
            var reporter = new EecalcValidationReporter();
            var expected = new EecalcExpectedSnapshot();
            var actual = new EecalcActualSnapshot();

            var result = reporter.Compare(expected, actual);

            Assert.True(result.Passed, reporter.Format(result));
        }
    }
}
