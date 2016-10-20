# Austeroids
Everyone's favorite asteroid game - powered by musical mushrooms.

![alt text](http://i.imgur.com/Quapn7h.png "Time to kick asteroids and chew mushrooms")

Demo video (turn down your volume!): http://host.foohy.net/files/zoomin.mp4

### What the heck
This is a very (very!) simple realtime game of asteroids, which includes movement, shooting, and destroying asteroids. But -- there's a catch, the application only outputs raw audio!
Using an X-Y oscilloscope to plot the left and right channels, you can see the actual gamefield!

Perhaps a better explanation can be offered in this video, using the same technique for a much larger and more musical display of visuals:    
https://www.youtube.com/watch?v=rtR63-ecUNo


### How the heck
The game field is represented as simply a series of points, which are converted into sampledata and slammed into an audio buffer, played back by Bass.Net. To keep latency down, the number of points are carefully controlled, and the sample rate is cranked up to 192,000 Hz.

### Further Information
* It's real prototypy. If you can, set your sound card's sample rate to 192,000 Hz, as that's what it was tested with.
* If you don't actually own an oscilloscope, I've made a program that uses your computer's audio output and plots it in realtime. Grab that here:
https://github.com/Foohy/Oscillofun
* It requires .Net 4.0 and a (relatively) quick CPU, in order to maintain the audio sample buffer.
