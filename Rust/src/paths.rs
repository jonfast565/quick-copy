use crate::utilities;

pub enum ActionType {
    Create,
    Update,
    Delete
}

pub const WindowsSplitter: char = '\\';
pub const UnixSplitter: char = '/';
pub const Splitter: char = '|';

#[derive(Clone)]
pub struct PathSegment {
    name: &'static str,
    next: Option<Box<PathSegment>>
}

impl PathSegment {
    fn get_remaining_segments(&self) -> Vec<PathSegment> {
        let current_segment = self;
        let mut results = Vec::<PathSegment>::new();
        results.push(current_segment.clone());
        let mut seg_option = current_segment.next.as_ref();
        while let Some(i) = seg_option {
            results.push(*i.clone());
            seg_option = i.next.as_ref();
        }
        results
    }

    fn get_segment_string(&self, separator: char) -> String {
        let remaining_segments = self.get_remaining_segments();
        let mut segment_string = String::new();
        for seg in remaining_segments {
            segment_string.push_str(seg.name);
            segment_string.push(separator);
        }
        segment_string.pop();
        segment_string
    }

    fn get_default_segment_string(&self) -> String {
        if cfg!(windows) {
            self.get_segment_string(WindowsSplitter)
        } else if cfg!(unix) {
            self.get_segment_string(UnixSplitter)
        } else {
            self.get_segment_string(UnixSplitter)
        }
    }

    fn get_segment_length(&self) -> usize {
        let segment_string = self.get_default_segment_string();
        let splitted : Vec<&str> = segment_string
            .split(Splitter)
            .collect();
        splitted.len()
    }

    fn contains_all_of_segment(&self, folder_segment: &PathSegment) -> bool {
        let str1 = self.get_segment_string(Splitter);
        let str2 = folder_segment.get_segment_string(Splitter);
        let split1 = str1.split(Splitter).collect::<Vec<&str>>();
        let split2 = str2.split(Splitter).collect::<Vec<&str>>();
        let mut split_ctr = 0;

        for t in split1 {
            if utilities::string_match_str(split2[split_ctr], t) {
                split_ctr += 1;

                if split_ctr == split2.len() {
                    return true
                }
            } else {
                split_ctr = 0;
            }
        }

        false
    }

    fn identical(&self, other_segment: &PathSegment) -> bool {
        let str1 = self.get_segment_string(Splitter);
        let str2 = other_segment.get_segment_string(Splitter);
        let split1 = str1.split(Splitter).collect::<Vec<&str>>();
        let split2 = str2.split(Splitter).collect::<Vec<&str>>();

        if split1.len() != split2.len() {
            return false;
        }

        for i in 0..split1.len() {
            if !utilities::string_match_str(split1[i], split2[i]) {
                return false
            }
        }
        
        true
    }
}