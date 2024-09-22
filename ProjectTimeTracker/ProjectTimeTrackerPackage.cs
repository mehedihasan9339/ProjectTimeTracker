using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace ProjectTimeTracker
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(ProjectTimeTrackerPackage.PackageGuidString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(TimeTrackerWindow))]
    public sealed class ProjectTimeTrackerPackage : AsyncPackage, IVsSolutionEvents
    {
        public const string PackageGuidString = "12345678-abcd-4321-abcd-1234567890ab";  // Replace with your actual GUID

        private IVsSolution _solution;
        private uint _solutionEventsCookie;
        private DateTime _projectStartTime;

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            _solution = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
            if (_solution != null)
            {
                _solution.AdviseSolutionEvents(this, out _solutionEventsCookie);
            }
            await TimeTrackerWindowCommand.InitializeAsync(this);
            await OpenTimeTrackerCommand.InitializeAsync(this);
        }

        // IVsSolutionEvents methods
        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            _projectStartTime = DateTime.Now;
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            var timeSpent = DateTime.Now - _projectStartTime;
            // Logic to save time spent can be placed here
            SaveTimeSpent(timeSpent);
            return VSConstants.S_OK;
        }

        private void SaveTimeSpent(TimeSpan timeSpent)
        {
            var projectTimeData = new ProjectTimeData
            {
                ProjectName = "MyProject",  // For demo, replace with the actual project name logic
                StartTime = _projectStartTime,
                TotalTimeSpent = timeSpent
            };

            TimeDataHelper.SaveTimeData(projectTimeData);
        }

        #region Unused IVsSolutionEvents Methods
        public int OnAfterCloseProject(IVsHierarchy pHierarchy, int fRemoved) => VSConstants.S_OK;
        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy) => VSConstants.S_OK;
        public int OnAfterMergeSolution(object pUnkReserved) => VSConstants.S_OK;
        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded) => VSConstants.S_OK;
        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved) => VSConstants.S_OK;
        public int OnBeforeCloseSolution(object pUnkReserved) => VSConstants.S_OK;
        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy) => VSConstants.S_OK;
        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel) => VSConstants.S_OK;
        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel) => VSConstants.S_OK;
        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel) => VSConstants.S_OK;
        #endregion
    }
}
