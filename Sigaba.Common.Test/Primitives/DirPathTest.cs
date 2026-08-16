using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.Primitives;

public sealed class DirPathTest
{
  private readonly MockFileSystem fs = new();



  // Exists

  [Fact]
  public void Should_know_if_it_exists_in_file_system()
  {
    fs.AddDirectory(fs.Path.Combine("a", "b", "c"));
    var existing = new DirPath(fs, "a", "b", "c");
    var nonExisting = new DirPath(fs, "a", "b", "d");

    existing.Exists.Should().BeTrue();
    nonExisting.Exists.Should().BeFalse();
  }


  // Parent

  [Fact]
  public void Should_get_parent_if_exists()
  {
    var dir = new DirPath(fs, "a", "b", "c");
    var expectedParent = new DirPath(fs, "a", "b");

    dir.Parent().Should().Be(expectedParent);
  }

  [Fact]
  public void Should_parent_should_be_null_if_no_parent_exists()
  {
    var dir = new DirPath(fs, "a");
    var parent = dir.Parent();

    parent.Should().BeNull();
  }

  // CombineAsDir

  [Fact]
  public void Should_combine_with_path_to_create_new_instance()

  {
    var initial = new DirPath(fs, "a");
    var expected = new DirPath(fs, "a", "b", "c");

    var actual = initial.CombineAsDir("b", "c");

    actual.Should().Be(expected);
  }

  // CombineAsFile

  [Fact]
  public void Should_combine_with_path_to_create_new_file_instance()
  {
    var dir = new DirPath(fs, "a");
    var expected = new FilePath(fs, "a", "b.txt");

    var actual = dir.CombineAsFile("b.txt");

    actual.Should().Be(expected);
  }

  // EnsureCreated

  [Fact]
  public void Should_create_folder_when_ensuring_creation()
  {
    var dir = new DirPath(fs, "a", "b", "c");

    dir.EnsureCreated();
    var action = () => dir.EnsureCreated();

    fs.Directory.Exists(dir.Path).Should().BeTrue();
    action.Should().NotThrow();
  }

  // TryGetNearestFileWithNameGoingUp

  [Fact]
  public void Should_get_first_file_with_a_name_going_up_the_folder_tree()
  {
    fs.AddDirectory(fs.Path.Combine("a", "b", "c", "d"));
    fs.AddEmptyFile(fs.Path.Combine("a", "file.txt"));
    fs.AddEmptyFile(fs.Path.Combine("a", "b", "file.txt"));
    var dir = new DirPath(fs, "a", "b", "c", "d");

    var result = dir.TryGetNearestFileWithNameGoingUp("file.txt", out var file);

    result.Should().BeTrue();
    file.Should().Be(new FilePath(fs, "a", "b", "file.txt"));
    file.Should().NotBe(new FilePath(fs, "a", "file.txt"));
  }

  [Fact]
  public void Should_get_null_if_first_file_with_a_name_going_up_does_not_exists()
  {
    fs.AddDirectory(fs.Path.Combine("a", "b", "c", "d"));
    var dir = new DirPath(fs, "a", "b", "c", "d");

    var result = dir.TryGetNearestFileWithNameGoingUp("file.txt", out var file);

    result.Should().BeFalse();
    file.Should().BeNull();
  }


}

