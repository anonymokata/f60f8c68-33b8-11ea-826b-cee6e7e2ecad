# Pencil-Durability-Kata

### Running Build and Tests

### Design choice Notes
  1) Point Degradation section made no mention on how to handle punctuation or numbers. Made degrading by one the default since these should still degrade the point when written. This also prevents any unaccounted characters from falling through the cracks and being written without dulling the point.
  1) Point Degradation section didn't have an example on how to treat uppercase letters if there was still durability left but not enough to write in uppercase. Decided this would cause a space to be written and to not cause any point degredation.
  1) Eraser Degradation seemed ambiguous on how to handle whitespace. Allowed removing it if it is explicitly put into a match for erasing but only if there is still durability on the eraser. Even though it does not degrade the eraser. If there is no eraser, nothing should be allowed to be erased.
