using MiniCrawler.Core;
using MiniCrawler.Progress;
using NUnit.Framework;
using UnityEngine;

namespace MiniCrawler.Tests
{
    public class RunSetupTests
    {
        private PartyMemberDefinition punchy;
        private PartyMemberDefinition healer;

        [SetUp]
        public void SetUp()
        {
            RunProgress.EndRun();

            punchy =
                TestPartyMemberFactory.Create(
                    "Punchy"
                );

            healer =
                TestPartyMemberFactory.Create(
                    "Healer"
                );
        }

        [TearDown]
        public void TearDown()
        {
            RunProgress.EndRun();

            Object.DestroyImmediate(punchy);
            Object.DestroyImmediate(healer);
        }

        [Test]
        public void NewSetup_StartsEmpty()
        {
            RunSetup setup = new RunSetup();

            Assert.That(
                setup.SelectedParty.Count,
                Is.Zero
            );
        }

        [Test]
        public void TogglePartyMember_AddsAndRemovesMember()
        {
            RunSetup setup = new RunSetup();

            Assert.That(
                setup.TogglePartyMember(punchy),
                Is.True
            );

            Assert.That(
                setup.IsSelected(punchy),
                Is.True
            );

            Assert.That(
                setup.SelectedParty.Count,
                Is.EqualTo(1)
            );

            Assert.That(
                setup.TogglePartyMember(punchy),
                Is.True
            );

            Assert.That(
                setup.IsSelected(punchy),
                Is.False
            );

            Assert.That(
                setup.SelectedParty.Count,
                Is.Zero
            );
        }

        [Test]
        public void Configuration_IsSnapshotOfSetup()
        {
            RunSetup setup = new RunSetup();

            setup.TogglePartyMember(punchy);

            RunStartConfiguration configuration =
                setup.CreateConfiguration();

            setup.TogglePartyMember(punchy);
            setup.TogglePartyMember(healer);

            Assert.That(
                configuration.Party.Count,
                Is.EqualTo(1)
            );

            Assert.That(
                configuration.Party[0],
                Is.SameAs(punchy)
            );
        }

        [Test]
        public void ActiveRun_IsNotChangedWhenSetupChanges()
        {
            RunSetup setup = new RunSetup();

            setup.TogglePartyMember(punchy);

            Assert.That(
                RunProgress.BeginRun(
                    setup.CreateConfiguration()
                ),
                Is.True
            );

            setup.TogglePartyMember(punchy);
            setup.TogglePartyMember(healer);

            Assert.That(
                RunProgress.SelectedParty.Count,
                Is.EqualTo(1)
            );

            Assert.That(
                RunProgress.SelectedParty[0],
                Is.SameAs(punchy)
            );
        }

        [Test]
        public void NewRun_UsesLatestSetup()
        {
            RunSetup setup = new RunSetup();

            setup.TogglePartyMember(punchy);

            Assert.That(
                RunProgress.BeginRun(
                    setup.CreateConfiguration()
                ),
                Is.True
            );

            RunProgress.EndRun();

            setup.TogglePartyMember(punchy);
            setup.TogglePartyMember(healer);

            Assert.That(
                RunProgress.BeginRun(
                    setup.CreateConfiguration()
                ),
                Is.True
            );

            Assert.That(
                RunProgress.SelectedParty.Count,
                Is.EqualTo(1)
            );

            Assert.That(
                RunProgress.SelectedParty[0],
                Is.SameAs(healer)
            );
        }
    }
}