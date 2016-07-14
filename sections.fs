\ Sections for the dictionary (like sections in assembly language)

\ Copyright (C) 2016 Free Software Foundation, Inc.

\ This file is part of Gforth.

\ Gforth is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public License
\ as published by the Free Software Foundation, either version 3
\ of the License, or (at your option) any later version.

\ This program is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\ GNU General Public License for more details.

\ You should have received a copy of the GNU General Public License
\ along with this program. If not, see http://www.gnu.org/licenses/.

\ !! ToDo: better integration with the rest of the system, in
\ particular the definitions or usages of FORTHSTART,
\ USABLE-DICTIONARY-END, DPP; and the usage for locals

\ Deal with MARKERs and the native code thingies

0
field: section-start \ during run-time
field: section-end
field: section-dp
constant section-desc

0 value sections
variable #sections
variable current-section \ index

256 1024 * value section-size

: section-addr ( i -- addr )
    section-desc * sections + ;

: current-section-addr ( -- addr )
    current-section @ section-addr ;

: hex.r ( u1 u2 -- )
    ['] .r #16 base-execute ;

: ann.sections { sections u curr-sect -- }
    cr sections #16 hex.r ."  start       end/offset               dp"
    u 0 u+do
        cr i curr-sect = if '>' else bl then emit
        i section-desc * sections +
        dup section-start @ #21 hex.r
        dup section-end   @ #17 hex.r
        section-dp        @ #17 hex.r
    loop ;

: .sections ( -- )
    sections #sections @ current-section @ ann.sections ;

: an.sections ( sections u -- )
    -1 ann.sections ;
    
: new-section ( -- )
    sections #sections @ section-desc * section-desc extend-mem drop to sections >r
    section-size allocate throw ( section-addr )
    dup r@ section-start !
    dup r@ section-dp !
    section-size + r> section-end !
    1 #sections +! ;

: set-section ( -- )
    \ any changes to other things after changing the section
    current-section-addr section-dp dpp ! ;

: next-section ( -- )
    \ switch to the next section, creating it if necessary
    1 current-section +!
    current-section @ #sections @ = if
	new-section
    then
    assert( current-section @ #sections @ < )
    set-section ;

: previous-section ( -- )
    \ switch to previous section
    assert( current-section @ 0> )
    -1 current-section +! set-section ;

\ initialization

: sections-ude ( -- addr )
    current-section-addr section-end @ ;

: init-sections ( -- )
    section-desc allocate throw to sections
    0 current-section !
    1 #sections !
    forthstart            sections section-start !
    usable-dictionary-end sections section-end !
    here                  sections section-dp !
    sections section-dp dpp ! \ !! dpp is reset to normal-dp on throw
    ['] sections-ude is usable-dictionary-end ;

:noname ( -- )
    init-sections
    defers 'cold ;
is 'cold

init-sections

\ savesystem

: dump-fi ( c-addr u -- )
    prepare-for-dump
    0 current-section ! set-section
    maxalign here { sect0-here }
    #sections @ 1 u+do
	i section-addr >r
	r@ section-start @ assert( dup dup maxaligned = )
	r@ section-dp @ maxaligned dup r> section-dp !
	over - save-mem-dict 2drop loop
    here forthstart - forthstart 2 cells + !
    here normal-dp ! 
    w/o bin create-file throw >r
    preamble-start here over - r@ write-file throw
    sect0-here sections section-dp !
    sections #sections @ section-desc * r@ write-file throw
    .sections cr
    #sections 1 cells r@ write-file throw
    r> close-file throw ;

[defined] testing [if]
.sections
cr next-section .sections
cr next-section .sections
cr previous-section .sections
cr previous-section .sections

next-section
: foo ." foo" ;
previous-section
: bar ." bar" ;
cr .sections
cr     
\ s" xxxsections" dump-fi
[then]
