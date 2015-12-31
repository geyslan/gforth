\ ansi.fs      Define terminal attributes              20may93jaw

\ Copyright (C) 1995,1996,1997,1998,2001,2003,2007,2013,2014,2015 Free Software Foundation, Inc.

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


\ If you want another terminal you can redefine the colours.

\ But a better way is it only to redefine SET-ATTR
\ to have compatible colours.

\ Attributes description:
\ <A ( -- -1 0 )           Start attributes description
\ A> ( -1 x .. x -- attr ) Terminate an attributes description and
\                          return overall attribute; currently only
\                          12 bits are used.
\
\ >BG ( colour -- x )      x is attribute with colour as Background colour
\ >FG ( colour -- x )      x is attribute with colour as Foreground colour
\
\ SET-ATTR ( attr -- )     Send attributes to terminal
\
\ BG> ( attr -- colour)    extract colour of Background from attr
\ FG> ( attr -- colour)    extract colour of Foreground from attr
\
\ See colorize.fs for an example of usage.

\ To do:        Make <A A> State smart and only compile literals!

require vt100.fs

decimal

0 $F xor Constant Black
1 $F xor Constant Red
2 $F xor Constant Green
3 $F xor Constant Yellow
4 $F xor Constant Blue
5 $F xor Constant Magenta
6 $F xor Constant Cyan
7 $F xor Constant White
9 $F xor Constant defaultcolor

1 CONSTANT Bold
2 CONSTANT Underline
4 CONSTANT Blink
8 CONSTANT Invers

\ For portable programs don't use invers and underline

: >BG    4 lshift ;
: >FG    8 lshift ;

: BG>    4 rshift 15 and ;
: FG>    8 rshift 15 and ;

: <A    -1 0 ;
: A>    BEGIN over -1 <> WHILE or REPEAT nip ;

User Attr   0 Attr !

: (Attr!) ( attr -- )
    \G set attribute
    dup Attr @ = IF drop EXIT THEN
    dup Attr !
    ESC[    0 pn
    dup FG> ?dup IF $F xor 30 + ;pn THEN
    dup BG> ?dup IF $F xor 40 + ;pn THEN
    dup Bold and IF 1 ;pn THEN
    dup Underline and IF 4 ;pn THEN
    dup Blink and IF 5 ;pn THEN
    Invers and IF 7 ;pn THEN
    [char] m emit ;

' (Attr!) IS Attr!

[IFDEF] debug-out
    debug-out op-vector !
    
    ' (Attr!) IS Attr!
    
    default-out op-vector !
[THEN]

: BlackSpace ( -- )
    Attr @ dup BG> Black =
    IF drop space
    ELSE 0 attr! space attr! THEN ;

