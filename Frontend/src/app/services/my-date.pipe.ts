import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({
  name: 'date'
})
export class MyDatePipe extends DatePipe implements PipeTransform {
  transform(value: any, args?: any): any {
    if (!args) {
      args = 'MMM d, y';
    }
    return super.transform(value, args);
  }

}
