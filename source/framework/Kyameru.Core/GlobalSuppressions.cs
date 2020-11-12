// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "Should not be serialized through to client.", Scope = "type", Target = "~T:Kyameru.Core.Exceptions.ActivationException")]
[assembly: SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "Should not be serialized through to client.", Scope = "type", Target = "~T:Kyameru.Core.Exceptions.ComponentException")]
[assembly: SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "Should not be serialized through to client.", Scope = "type", Target = "~T:Kyameru.Core.Exceptions.ProcessException")]