/*
   Copyright 2016 - 2017 Adrian Popescu.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;
using Xunit;

namespace Tests
{
    [Collection(Keywords.REDMINE_MANAGER_COLLECTION)]
    public class Tests
    {
        public Tests(RedmineManagerFixture fixture)
        {
            this.fixture = fixture;
        }

        private const string PROJECT_IDENTIFIER = "redmine-net-api-project-test";
        private const string PROJECT_NAME = "Redmine Net Api Project Test";

        private readonly RedmineManagerFixture fixture;

        private readonly string NewProjectId = "rnapwps" + Guid.NewGuid();
        private readonly string NewParentProjectId = "parent-project" + Guid.NewGuid();

        private static Project CreateTestProjectWithRequiredPropertiesSet()
        {
            var project = new Project
            {
                Name = PROJECT_NAME,
                Identifier = PROJECT_IDENTIFIER
            };

            return project;
        }

        private static Project CreateTestProjectWithAllPropertiesSet()
        {
            var project = new Project
            {
                Name = "Redmine Net Api Project Test All Properties",
                Description = "This is a test project.",
                Identifier = "rnaptap",
                HomePage = "www.redminetest.com",
                IsPublic = true,
                InheritMembers = true,
                Status = ProjectStatus.Active,
                EnabledModules = new List<ProjectEnabledModule>
                {
                    new ProjectEnabledModule {Name = "issue_tracking"},
                    new ProjectEnabledModule {Name = "time_tracking"}
                },
                Trackers = new List<ProjectTracker>
                {
                    new ProjectTracker {Id = 1},
                    new ProjectTracker {Id = 2}
                }
            };

            return project;
        }

        private static Project CreateTestProjectWithInvalidTrackersId()
        {
            var project = new Project
            {
                Name = "Redmine Net Api Project Test Invalid Trackers",
                Identifier = "rnaptit",
                Trackers = new List<ProjectTracker>
                {
                    new ProjectTracker {Id = 999999},
                    new ProjectTracker {Id = 999998}
                }
            };

            return project;
        }

        private static Project CreateTestProjectWithParentSet(int parentId)
        {
            var project = new Project
            {
                Name = "Redmine Net Api Project With Parent Set",
                Identifier = "rnapwps",
                Parent = new IdentifiableName { Id = parentId }
            };

            return project;
        }

        private async Task Delete<T>(string id) where T : class, new()
        {
            //var exception = await Record.ExceptionAsync(() => fixture.RedmineManager.Delete<T>(id));
            //Assert.Null(exception);
           // Assert.(exception != null, $"Delete {id} - {exception.Message}");
           // await Assert.ThrowsAsync<RedmineException>(() => fixture.RedmineManager.Get<T>(id, null));
        }

        [Fact]
        public async void Should_Create_Project_With_All_Properties_Set()
        {
            var savedProject = await fixture.RedmineManager.Create(CreateTestProjectWithAllPropertiesSet());

            Assert.NotNull(savedProject);
            Assert.NotEqual(savedProject.Id, 0);
            Assert.True(savedProject.Identifier.Equals("rnaptap"), "Project identifier is invalid.");
            Assert.True(savedProject.Name.Equals("Redmine Net Api Project Test All Properties"), "Project name is invalid.");
        }

        [Fact]
        public async void Should_Create_Project_With_Parent()
        {
            var parentProject = await fixture.RedmineManager.Create(new Project { Identifier = "parent-project", Name = "Parent project" });
            var savedProject = await fixture.RedmineManager.Create(CreateTestProjectWithParentSet(parentProject.Id));

            Assert.NotNull(savedProject);
            Assert.True(savedProject.Parent.Id == parentProject.Id, "Parent project is invalid.");
        }

        [Fact]
        public async void Should_Create_Project_With_Required_Properties()
        {
            var savedProject = await fixture.RedmineManager.Create(CreateTestProjectWithRequiredPropertiesSet());

            Assert.NotNull(savedProject);
            Assert.NotEqual(savedProject.Id, 0);
            Assert.True(savedProject.Name.Equals(PROJECT_NAME), "Project name is invalid.");
            Assert.True(savedProject.Identifier.Equals(PROJECT_IDENTIFIER), "Project identifier is invalid.");
        }

        [Fact]
        public async void Should_Delete_Project_And_Parent_Project()
        {
            await Delete<Project>(NewProjectId);
            await Delete<Project>(NewParentProjectId);
        }

        [Fact]
        public async void Should_Delete_Project_With_All_Properties_Set()
        {
            Func<Task> t = () => Task.Run(() => fixture.RedmineManager.Delete<Project>("rnaptap"));
            var exception = await Record.ExceptionAsync(t);
            Assert.Null(exception);
            await Assert.ThrowsAsync<RedmineException>(() => fixture.RedmineManager.Get<Project>("rnaptap", null));
        }

        [Fact]
        public async void Should_Delete_Redmine_Net_Api_Project_Test_Project()
        {
            var exception = await Record.ExceptionAsync(() => fixture.RedmineManager.Delete<Project>(PROJECT_IDENTIFIER));
            Assert.Null(exception);
            await Assert.ThrowsAsync<RedmineException>(() => fixture.RedmineManager.Get<Project>(PROJECT_IDENTIFIER, null));
        }

        [Fact]
        public async void Should_Get_All_Projects()
        {
            var projects = await fixture.RedmineManager.ListAll<Project>(null);
            Assert.NotNull(projects);
        }

        [Fact]
        public async void Should_Get_Paged_Projects()
        {
            var projects = await fixture.RedmineManager.List<Project>(new NameValueCollection { { RedmineKeys.LIMIT, "2" } });
            Assert.NotNull(projects);
        }

        [Fact]
        public async void Should_Get_Redmine_Net_Api_Project_Test_Project()
        {
            var project = await fixture.RedmineManager.Get<Project>(PROJECT_IDENTIFIER, null);

            Assert.NotNull(project);
            Assert.IsType<Project>(project);
            Assert.Equal(project.Identifier, PROJECT_IDENTIFIER);
            Assert.Equal(project.Name, PROJECT_NAME);
        }

        [Fact]
        public async void Should_Get_Test_Project_With_All_Properties_Set()
        {
            var project = await fixture.RedmineManager.Get<Project>("rnaptap", new NameValueCollection
            {
                {RedmineKeys.INCLUDE, string.Join(",", RedmineKeys.TRACKERS, RedmineKeys.ENABLED_MODULES)}
            });

            Assert.NotNull(project);
            Assert.IsType<Project>(project);
            Assert.True(project.Name.Equals("Redmine Net Api Project Test All Properties"), "Project name not equal.");
            Assert.True(project.Identifier.Equals("rnaptap"), "Project identifier not equal.");
            Assert.True(project.Description.Equals("This is a test project."), "Project description not equal.");
            Assert.True(project.HomePage.Equals("www.redminetest.com"), "Project homepage not equal.");
            //Assert.True(project.IsPublic.Equals(true), "Project is_public not equal. (This property is available starting with 2.6.0)");

            Assert.True(project.Trackers != null, "Trackers are null!");
            Assert.True(project.Trackers.Count == 2, $"Trackers found ({project.Trackers.Count}) != Trackers expected (2)");
            Assert.All(project.Trackers, t => Assert.IsType<ProjectTracker>(t));

            Assert.True(project.EnabledModules != null, "Enabled modules is null!");
            Assert.True(project.EnabledModules.Count == 2, $"Enabled modules found ({project.EnabledModules.Count}) != Enabled modules expected (2)");
            Assert.All(project.EnabledModules, em => Assert.IsType<ProjectEnabledModule>(em));
        }

        [Fact]
        public async void Should_Throw_Exception_Create_Project_Invalid_Trackers()
        {
            await Assert.ThrowsAsync<UnprocessableEntityException>(() => fixture.RedmineManager.Create(CreateTestProjectWithInvalidTrackersId()));
        }

        [Fact]
        public async void Should_Throw_Exception_When_Create_Empty_Project()
        {
            await Assert.ThrowsAsync<UnprocessableEntityException>(() => fixture.RedmineManager.Create(new Project()));
        }

        [Fact]
        public async void Should_Throw_Exception_When_Project_Identifier_Is_Invalid()
        {
            await Assert.ThrowsAsync<RedmineException>(() => fixture.RedmineManager.Get<Project>("99999999", null));
        }

        [Fact]
        public async void Should_Update_Redmine_Net_Api_Project_Test_Project()
        {
            const string UPDATED_PROJECT_NAME = "Project created using API updated";
            const string UPDATED_PROJECT_DESCRIPTION = "Test project description updated";
            const string UPDATED_PROJECT_HOMEPAGE = "http://redmineTestsUpdated.com";
            const bool UPDATED_PROJECT_ISPUBLIC = true;
            const bool UPDATED_PROJECT_INHERIT_MEMBERS = false;

            var project = await fixture.RedmineManager.Get<Project>(PROJECT_IDENTIFIER, null);

            project.Name = UPDATED_PROJECT_NAME;
            project.Description = UPDATED_PROJECT_DESCRIPTION;
            project.HomePage = UPDATED_PROJECT_HOMEPAGE;
            project.IsPublic = UPDATED_PROJECT_ISPUBLIC;
            project.InheritMembers = UPDATED_PROJECT_INHERIT_MEMBERS;

            var exception = await Record.ExceptionAsync(() => fixture.RedmineManager.Update(PROJECT_IDENTIFIER, project));

            Assert.Null(exception);

            var updatedProject = await fixture.RedmineManager.Get<Project>(PROJECT_IDENTIFIER, null);

            Assert.True(updatedProject.Name.Equals(UPDATED_PROJECT_NAME), "Project name was not updated.");
            Assert.True(updatedProject.Description.Equals(UPDATED_PROJECT_DESCRIPTION), "Project description was not updated.");
            Assert.True(updatedProject.HomePage.Equals(UPDATED_PROJECT_HOMEPAGE), "Project homepage was not updated.");
            //  Assert.True(updatedProject.IsPublic.Equals(UPDATED_PROJECT_ISPUBLIC), "Project is_public was not updated. (This property is available starting with 2.6.0)");
        }
    }
}