The input is a large text file, where each line is of the form 'Number. String'

For example:
  415\. Apple
  30432\. Something something something
  1\. Apple
  32\. Cherry is the best
  2\. Banana is yellow

Both parts can be repeated within the file. You need to get another file at the output, where all the lines are sorted. Sorting criterion: the first part of String is compared first, if it matches, then Number.
Those in the example above it should be:
  1\. Apple
  415\. Apple
  2\. Banana is yellow
  32\. Cherry is the best
  30432\. Something something something

You need to write two programs:
  1. A utility for creating a test file of a given size. The result of the work should be a text file of the type described above. There must be some number of strings with the same String part.
  2. The actual sorter. An important point, the file can be very large. The size of ~100Gb will be used for testing.

Time reference - a 10 GB file is sorted in about 9 minutes (can be faster), and a 1 GB file is sorted within a minute (the fastest result is 26 seconds).
As an additional reference point - when sorting 1 GB, 2-2.5 GB of memory is used.
