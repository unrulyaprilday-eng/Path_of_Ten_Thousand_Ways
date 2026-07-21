using System;
using System.Collections.Generic;
using System.Linq;

namespace PathOfTenThousandWays.Demo.Systems
{
    public enum DemoCorePracticeType
    {
        MindMethod,
        BodyCultivation
    }

    public enum DemoTechniqueKind
    {
        HeartFormula,
        SwordArt,
        BodyArt,
        FistArt,
        Spell,
        Movement
    }

    public enum DemoBearerMode
    {
        Ground,
        Artifact,
        Handheld,
        Orbiting,
        BackMounted
    }

    public sealed class DemoCorePracticeDefinition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DemoCorePracticeType PracticeType { get; set; }
        public string PassiveRuleText { get; set; }
        public string GrantedTechniqueId { get; set; }
        public string SourceStoryId { get; set; }

        public IReadOnlyList<string> Validate()
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Id))
            {
                errors.Add("core practice id is required");
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                errors.Add(Id + " name is required");
            }

            if (string.IsNullOrWhiteSpace(GrantedTechniqueId))
            {
                errors.Add(Id + " must grant one active technique");
            }

            if (string.IsNullOrWhiteSpace(SourceStoryId))
            {
                errors.Add(Id + " must declare a story source");
            }

            return errors;
        }
    }

    public sealed class DemoTechniqueDefinition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DemoTechniqueKind Kind { get; set; }
        public string SourceStoryId { get; set; }
        public string RulesText { get; set; }
        public string VisualTag { get; set; }

        public IReadOnlyList<string> Validate()
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Id))
            {
                errors.Add("technique id is required");
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                errors.Add(Id + " name is required");
            }

            if (string.IsNullOrWhiteSpace(SourceStoryId))
            {
                errors.Add(Id + " must declare a story source");
            }

            return errors;
        }
    }

    public sealed class DemoBearerDefinition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DemoBearerMode Mode { get; set; }
        public string ResourceKey { get; set; }
        public bool IsRequired { get; set; }

        public IReadOnlyList<string> Validate()
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Id))
            {
                errors.Add("bearer id is required");
            }

            if (Mode == DemoBearerMode.Artifact && string.IsNullOrWhiteSpace(ResourceKey))
            {
                errors.Add(Id + " artifact bearer requires a resource key");
            }

            return errors;
        }
    }

    public sealed class DemoStartingPracticePackageDefinition
    {
        public string Id { get; set; }
        public string RootId { get; set; }
        public string SourceStoryId { get; set; }
        public string InnateArtifactId { get; set; }
        public string CorePracticeId { get; set; }
        public string PrimaryTechniqueId { get; set; }
        public string BearerDefinitionId { get; set; }
        public bool IsAvailable { get; set; }
        public List<string> ActiveTechniqueIds { get; set; } = new List<string>();

        public IReadOnlyList<string> Validate(
            DemoCorePracticeDefinition corePractice,
            IReadOnlyDictionary<string, DemoTechniqueDefinition> techniques,
            DemoBearerDefinition bearer)
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Id))
            {
                errors.Add("starting package id is required");
            }

            if (string.IsNullOrWhiteSpace(SourceStoryId))
            {
                errors.Add(Id + " must declare a story source");
            }

            if (corePractice == null || !string.Equals(corePractice.Id, CorePracticeId, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(Id + " references a missing core practice");
            }
            else
            {
                errors.AddRange(corePractice.Validate());
                if (!ActiveTechniqueIds.Contains(corePractice.GrantedTechniqueId, StringComparer.OrdinalIgnoreCase))
                {
                    errors.Add(Id + " must include the core practice granted technique");
                }
            }

            if (techniques == null)
            {
                errors.Add(Id + " requires a technique catalog");
            }
            else
            {
                foreach (string techniqueId in ActiveTechniqueIds.Concat(new[] { PrimaryTechniqueId }))
                {
                    if (string.IsNullOrWhiteSpace(techniqueId) || !techniques.ContainsKey(techniqueId))
                    {
                        errors.Add(Id + " references a missing technique: " + techniqueId);
                    }
                }
            }

            if (bearer == null || !string.Equals(bearer.Id, BearerDefinitionId, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(Id + " references a missing bearer");
            }

            if (ActiveTechniqueIds.Count != 2
                || ActiveTechniqueIds.Distinct(StringComparer.OrdinalIgnoreCase).Count() != 2
                || string.IsNullOrWhiteSpace(PrimaryTechniqueId)
                || !ActiveTechniqueIds.Contains(PrimaryTechniqueId, StringComparer.OrdinalIgnoreCase))
            {
                errors.Add(Id + " must expose exactly two distinct active techniques including the primary technique");
            }

            return errors;
        }

        public static DemoStartingPracticePackageDefinition SwordDemo()
        {
            return new DemoStartingPracticePackageDefinition
            {
                Id = "package_branch_sword_embryo",
                RootId = "root_branch",
                SourceStoryId = "opening_story_branch_ancestral_vault",
                InnateArtifactId = "artifact_broken_sword_embryo",
                CorePracticeId = "practice_branch_breathing",
                PrimaryTechniqueId = "technique_incomplete_sword_scroll",
                BearerDefinitionId = "bearer_old_mine_ground",
                IsAvailable = true,
                ActiveTechniqueIds = new List<string>
                {
                    "technique_breathing_recovery",
                    "technique_incomplete_sword_scroll"
                }
            };
        }
    }
}
