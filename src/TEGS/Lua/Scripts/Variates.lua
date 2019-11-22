-- 
-- Variates.lua
--  
-- Author:
--       Jon Thysell <thysell@gmail.com>
-- 
-- Copyright (c) 2019 Jon Thysell <http://jonthysell.com>
-- 
-- Permission is hereby granted, free of charge, to any person obtaining a copy
-- of this software and associated documentation files (the "Software"), to deal
-- in the Software without restriction, including without limitation the rights
-- to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
-- copies of the Software, and to permit persons to whom the Software is
-- furnished to do so, subject to the following conditions:
-- 
-- The above copyright notice and this permission notice shall be included in
-- all copies or substantial portions of the Software.
-- 
-- THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
-- IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
-- FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
-- AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
-- LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
-- OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
-- THE SOFTWARE.

function t_uniformvariate(alpha, beta)
    alpha = alpha or 0.0
    beta = beta or 1.0
    return alpha + (beta - alpha) * math.random()
end

function t_expovariate(lambda)
    return (-math.log(1.0 - t_uniformvariate()) / lambda)
end

function t_normalvariate(mu, sigma)
    local z = 0
    while(true)
    do
        local u1 = t_uniformvariate()
        local u2 = 1.0 - t_uniformvariate()
        z = (4 * math.exp(-0.5) / math.sqrt(2.0)) * (u1 - 0.5) / u2
        local zz = z * z / 4.0
        if zz <= -math.log(u2) then break end
    end 
    return mu + z * sigma
end

function t_lognormalvariate(mu, sigma)
    return math.exp(t_normalvariate(mu, sigma))
end
