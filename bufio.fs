\ BUFOUT.STR    Buffered output for Debug               13jun93jaw

\ Copyright (C) 1995,1996,1997 Free Software Foundation, Inc.

\ This file is part of Gforth.

\ Gforth is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public License
\ as published by the Free Software Foundation; either version 2
\ of the License, or (at your option) any later version.

\ This program is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\ GNU General Public License for more details.

\ You should have received a copy of the GNU General Public License
\ along with this program; if not, write to the Free Software
\ Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.

CREATE O-Buffer 4000 chars allot align
VARIABLE O-PNT

: O-TYPE        O-PNT @ over chars O-PNT +!
                swap move ;

: O-EMIT        O-PNT @ c! 1 chars O-PNT +! ;

VARIABLE EmitXT
VARIABLE TypeXT

: O-INIT        What's type TypeXT !
                What's emit EmitXT !
                O-Buffer O-PNT !
                ['] o-type IS type
                ['] o-emit IS emit ;

: O-DEINIT      EmitXT @ IS Emit
                TypeXT @ IS Type ;

: O-PNT@        O-PNT @ O-Buffer - ;

